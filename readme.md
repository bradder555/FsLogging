# Introduction
There are other logging libraries out there, for some reason, they all seem to use reflection and do 'weird stuff' behind the scenes.

This library is the minimum of what i think a logging library should do

There are very simple interfaces that the user must adhere to, to add their own 'logging endpoints' to the logger

The usage is very 'free' the logging endpoints know of the log-level of a message, thus one could add multiple loggers that filter on one or two endpoints etc
