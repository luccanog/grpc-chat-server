# grpc-chat-server

<img src='https://github.com/luccanog/grpc-chat-server/assets/55986783/bc925aa5-19e1-4bd4-b390-148c54ef5e40' width='256'>

## Overview

grpc-chat-server is a backend application that provides a chat server where users (clients) can create rooms and engage in real-time chat conversations. This project is built using .NET 7 and leverages various technologies such as Docker for containerization, gRPC for communication, Serilog for logging, xUnit for testing, and Moq for mocking.

## Comming next
* Deployed/Hosted Backend server
* Online frontend project to interact with

## Features

- Create chat rooms
- Join chat rooms
- Send and receive real-time messages within chat rooms

## Info
- Containerized using Docker
- Logging using Serilog
- Test-driven development with xUnit and Moq

## Prerequisites

Before you begin, ensure you have the following requirements in place:

- [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Docker](https://www.docker.com/) (for containerization)
- Basic knowledge of gRPC

## Installation

1. Clone this repository to your local machine:
```bash
git clone https://github.com/yourusername/grpc-chat-server.git
```
2. Change to the project directory:
```bash
 cd grpc-chat-server
```
3. Build the Docker container:
```bash
docker build -t grpc-chat-server .
```
4. Run the Docker container:
```bash
docker run grpc-chat-server
```

##
<a href="https://iconscout.com/icons/chat" class="text-underline font-size-sm" target="_blank">Free Chat  Icon</a> by <a href="https://iconscout.com/contributors/iconic" class="text-underline font-size-sm" target="_blank">Iconic .</a>
