# Messaging Application Documentation

This documentation provides an overview of the messaging application implemented in the `Program` class. It describes the application's purpose, initialization process, message consumption and production, as well as logging functionality. The documentation aims to assist future developers in understanding the code and its functionality.

## Table of Contents

- [Introduction](#introduction)
- [Initialization](#initialization)
- [Message Consumption](#message-consumption)
- [Message Production](#message-production)
- [Logging](#logging)

## Introduction

The messaging application is designed to facilitate communication between a client application and a service. It uses RabbitMQ as the message broker and implements a simple message exchange mechanism.

The application consists of a client application and a service. The client sends messages to the service, which processes and responds to them. The `Program` class is responsible for initializing the application, handling message consumption from the service, sending messages to the service, and logging relevant events.

## Initialization

The initialization section of the code sets up the connection to the RabbitMQ server. It defines a class-level variable `_connection` of type `IConnection` and `_channel` of type `IModel` to interact with the RabbitMQ server. The `InitializeConnection` method establishes the connection and creates the channel.

The RabbitMQ server is assumed to be running on the local machine, with the hostname set to `localhost`.

## Message Consumption

The application consumes messages sent by the service using the `StartMessageConsumption` method. This method declares a queue named `_serviceToApplicationQueue` and sets up a message consumer to receive messages from this queue.

The `ProcessReceivedMessage` method is triggered when a message is received. It retrieves the message from the `BasicDeliverEventArgs` object, logs the received message, and acknowledges the receipt by calling `_channel.BasicAck`.

## Message Production

The application can send messages to the service using the `SendMessage` method. By default, the method sends the message "Hello, Service!" to the service.

The `SendMessageToClientAfterDelay` method continuously sends the specified message to the service after a specified time interval. It uses the `SendMessage` method and awaits a delay using `Task.Delay`. The default delay is set to 30 seconds.

The application declares a queue named `_applicationToServiceQueue` before sending the message to the service. It then publishes the message using `_channel.BasicPublish`.

## Logging

The application provides logging functionality using the `LogMessage` method. It formats log messages with a timestamp and the provided message. The formatted message is then displayed on the console.

## Usage

To use the messaging application, follow these steps:

1. Set up RabbitMQ server and ensure it is running on the local machine.
2. Update the `HostName` property in the `InitializeConnection` method if the RabbitMQ server is not running on `localhost`.
3. Run the application.
4. The application will start consuming messages from the service and display them in the console.
5. Press any key to exit the application.

## Conclusion

This documentation provides an overview of the messaging application implemented in the `Program` class. It explains the initialization process, message consumption, message production, and logging functionality. Understanding this documentation will assist future developers in working with the messaging application and extending its functionality as required.
