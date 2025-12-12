# DATABASPROJEKT
A simple database project using SQL and C#.

## Features
- [Overview](#overview)
- [Description](#description)
- [Features](#features)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Usage](#usage)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Entity Relations](#entity-relations)
- [License](#license)
- [Screenshots](#screenshots)

## Overview

This project is a database application in EF Core for an e-store developed within the Database Development course and aims to provide practical experience in:

* ER modeling and normalization
* SQL/SQLite and constraints
* Entity Framework Core (EF Core)
* CRUD functionality
* Basic encryption
* Performance measurement and optimization
  
## Description

Performance measurement and optimization. The content of this database consists of models for customers, products and categories, among other things. All models are built with respective related relationships, such as Foreign Keys and Primary Keys. 
The database has a seeding with information on customers, products and more. The database is navigated with CLI commands to get a simple form of navigation in switches. In order for this database to have some form of security, I use an “Encryption” system which in this case is stored as a hash with salt.

- Possible/known limitations of the program

* Order seed was removed, due to relationship issues, but new orders can be created correctly.
* Status should perhaps be available as more options than currently available.
* No admin system (anyone can delete everything).
* No role-based access control implemented.
* The status fields could be developed with more values.

## Features

- User Authentication: Secure password lock for sensetive customer info.
- Editing of details or for an example viewing a specific customer order.
- Deletetion of data information
- Data Management: CRUD operations for managing data.

Trigger
A SQLite trigger is used to check that the stock balance does not become negative when placing an order. In case of error, a rollback is made.

View
A view is created to summarize order information, such as total amount, customer name, and is used in the application.

## Getting Started

## Prerequisites

* Visual studio or similar coding program
* Newer version of windows

## Installation

1. Clone the repository: https://github.com/Mcfluffelbl/DATABASPROJEKT.git
   ```bash
   git clone
2. Make sure you have the correct NuGet package - (Version 9.0.11)
3. Run the application or (dotnet run) - 
   ```bash
   dotnet run
   ```
4. Now navigate around as desired. (To see the customer's respective details, you must have access to their password) | Customer : DaVinci - Password: “1234” | Customer : Sten - Password: “4321”
5. ```bash
   1234
   ```

## Usage

1. Launch the application.
2. Register a new user or log in with existing credentials.
3. Navigate through the application to manage your data.

## Configuration


## Project Structure

CRUD Functionality -
The system has full CRUD flows for:

Customers -
* View all customers
* Create customer (validation: name, email, password are required)
* Update customer
* Delete customer
* View customer details (password required → verified against hash)

Products & Categories -
* Full CRUD and listing
* Sort by name, price or category
* Price validation (must be > 0)

Orders -
* Create order with order lines
* Transaction for the entire order flow
* Rollback on error (e.g. insufficient stock balance)
* Update order status
* List all orders with pagination and sorting

## Entity Relations

<img width="818" height="591" alt="Skärmbild 2025-12-12 030331" src="https://github.com/user-attachments/assets/11353ba4-f51d-4056-8e33-168ed5a2cf6b" />

ER-diagram:
Customer (1) —— (0..N) Order
Order (1) —— (1..N) OrderRow
OrderRow (N) —— (1) Product
Product (N) —— (1) Category

## License
This project is not licensed.
You may modify or use it at your own discretion.

## Screenshots

1. ( Main Menu of the program )
<img width="1108" height="356" alt="Skärmbild 2025-12-12 032403 1" src="https://github.com/user-attachments/assets/30e4fb72-354a-432f-9ba2-27e0ad5c1560" />

2. ( Encrypted Customer info )
<img width="1122" height="602" alt="Skärmbild 2025-12-12 032450 2" src="https://github.com/user-attachments/assets/a5415aba-7e1c-4592-9271-66114f956a4a" />

3. ( Decrypted Customer info by use of the customers password )
<img width="1095" height="610" alt="Skärmbild 2025-12-12 032513 3" src="https://github.com/user-attachments/assets/e7a2720a-0184-403f-8d60-85cff47c98c9" />

4. ( Example method/function of the application )
<img width="1094" height="601" alt="Skärmbild 2025-12-12 032544 4" src="https://github.com/user-attachments/assets/a0856ab3-c9d2-425a-84a5-515f4daf768d" />



