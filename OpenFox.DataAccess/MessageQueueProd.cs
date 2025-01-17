using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using OpenFox.Logging;

namespace OpenFox.DataAccess
{

    public class MessageQueueProd : IMessageQueue
    {
        private readonly string _connectionString;
        ILogger _logger;
        
        //start at 10, to reserve some numbers, 1 is connection, 2 is heartbeat.
        private readonly SequentialUShortGenerator _sequentialUShortGenerator = new SequentialUShortGenerator(10);
        private readonly Dictionary<ushort, int> _exchangeIdToDbId = new Dictionary<ushort, int>();

        public MessageQueueProd(string connectionString, ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public OFMLRequest GetNextMessage()
        {
            OFMLRequest request = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand("select_next_message", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int dbId = reader.GetInt32(reader.GetOrdinal("pk"));
                            string text = reader.GetString(reader.GetOrdinal("msg"));
                            ushort exchangeId = CreateExchangeIdFromDbId(dbId);

                            request = new OFMLRequest()
                            {
                                id = exchangeId,
                                text = text
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }


            return request;

        }

        public void MarkMessageSent(ushort exchangeId)
        {
            int dbId = GetDbIdFromExchangeId(exchangeId);
            if (dbId == 0)
            {
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand("mark_message_sent", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@pk", dbId);
                    connection.Open();
                    command.ExecuteNonQuery();

                    ClearExchange(exchangeId); // clear it in dictionary just to tidy up
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        public bool SaveResponse(OFMLResponse response)
        {
            int insertCount = 0;

            // Employee number who sent the message. Responses do not have referential exchangeId to match sender by that.
            string userId = response.userId;

            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("insert_response", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@userId", userId); // can be multiple responses
                    command.Parameters.AddWithValue("@msg", response.message.Replace("'", "''"));
                    command.Parameters.AddWithValue("@source", response.source ?? "");
                    insertCount = command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            return insertCount > 0;
        }

        private ushort CreateExchangeIdFromDbId(int dbId)
        {
            ushort exchangeId = _sequentialUShortGenerator.Next();
            _exchangeIdToDbId[exchangeId] = dbId;
            return exchangeId;
        }

        private int GetDbIdFromExchangeId(ushort exchangeId)
        {
            int valueForKey;
            if (_exchangeIdToDbId.TryGetValue(exchangeId, out valueForKey))
            {
                return valueForKey;
            }
            return 0;
        }

        private void ClearExchange(ushort exchangeId)
        {
            _exchangeIdToDbId.Remove(exchangeId);
        }
    }


    public class SequentialUShortGenerator
    {
        private ushort currentValue;
        private readonly ushort minValue;
        private readonly ushort maxValue;

        public SequentialUShortGenerator(ushort min, ushort max = ushort.MaxValue)
        {
            if (min >= max)
            {
                throw new ArgumentException("Invalid range: minValue must be less than maxValue");
            }

            this.minValue = min;
            this.maxValue = max;
            this.currentValue = min;
        }

        public ushort Next()
        {
            // Check for rollover
            if (currentValue == maxValue)
            {
                currentValue = minValue;
            }
            else
            {
                currentValue++;
            }

            return currentValue;
        }
    }
}

