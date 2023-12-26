# OperaGX-Nitro-Generator

![License](https://img.shields.io/badge/license-MIT-blue)

Last Updated On: December 26, 2023

## Overview

OperaGX-Nitro-Generator is a .NET console application designed for efficient and high-speed interaction with the Discord API for partner promotions. This project leverages web proxy functionality and multithreading to enhance performance, allowing for rapid generation of requests.
**With the ability to reach speeds of up to 100,000 requests in just one and a half minutes under optimal conditions**, OperaGX-Nitro-Generator is a standout solution for automating Discord-related tasks.

## Features

- **Web Proxy Integration**: Generator supports web proxy usage, allowing users to route their requests through proxies for enhanced security, anonymity, and versatility.

- **Multithreading**: The application utilizes multithreading to perform asynchronous HTTP requests concurrently. This approach significantly improves the speed of request execution and ensures efficient resource utilization.

- **Configurability**: Generator is easily configurable, allowing users to specify the number of requests per iteration, the number of iterations, and the delay between requests. This flexibility ensures adaptability to various use cases.

- **Service Dependency Injection**: The project adopts the Microsoft.Extensions.DependencyInjection framework, facilitating the management of services and promoting a clean and modular code structure.

## How to Use

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/sweeperxz/OperaGX-Nitro-Generator-Fast.git
   ```
2. **Build and Run:**

   ```bash
   cd OperaGX-Nitro-Generator-Fast
   dotnet build
   dotnet run
   ```

3. **Follow On-Screen Instructions:**

- Input the desired parameters for the number of requests, iterations, and delay between requests.
- Observe the application's execution and completion progress.
