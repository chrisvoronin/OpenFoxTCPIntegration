# OpenFox Integration

OF .net client built to specification.
Built on system requirements of c# .net 4.8

** How it works **
App attempts to connect to OpenFox for message exchange.  Upon establishing connection and acknowledgement it will send messages one at a time to OF and read responses per system requirements.

**Message Responses**

Message responses come over multiple frames over TCP and are stitched together to make a complete message till the ending is read.

**Message Storage**

Built specifically for message stored around previous OF SQL server versions. Test version doesn't store anything.