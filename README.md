# Zero-Knowledge Encrypted Notes Demo

This is a secure note-taking application that demonstrates client-side, zero-knowledge encryption of user content using .NET and Blazor. All notes are encrypted before leaving your device, and the system is designed so that the server never has access to any password or decrypted data.

---

## Purpose

This project is for demonstrative purposes only and is **not intended for real-world, security-sensitive use**.  
It showcases a layered, zero-knowledge encrypted notes application built with .NET and Blazor.  
It uses **BlazorSodium** for cryptography in the client and **Sodium.Core** for cryptography on the server side.

---

## Zero-Knowledge Security Model

- **Client-side encryption:** All encryption and decryption are performed in the browser.
- **Password privacy:** Passwords are never sent to the server.
- **Authentication:** Login uses a challenge-response protocol with a public/private key pair and nonce signature. The server authenticates users by verifying a signed nonce, never by transmitting or storing passwords.
- **End-to-end encryption:** All notes are encrypted using keys derived from the user's password on the client side. Only encrypted data is ever transmitted or stored.

---

## Features

- **User authentication:** Signup and login via public/private key challenge-response.
- **End-to-end encryption:** All note data is encrypted client-side using BlazorSodium.
- **Note management:** Create, view, update, and delete encrypted notes.
- **Layered architecture:** Clear separation of API, business logic, data access, and client code.
- **Testing:** Includes tests for repository and service layers.

---

## Technologies

- **.NET 8 / Blazor**
- **Entity Framework Core**
- **BlazorSodium** (client-side cryptography)
- **Sodium.Core** (server-side cryptography)

---

## Getting Started

1. **Clone the repository:**
   ```sh
   git clone https://github.com/ExtendedGuru4883/EncryptedNotes.git
   ```

2. **Add required `appsettings.json` files:**

    - For the **API project**, you need an `appsettings.json` file with the following:
        - A database connection string named `DefaultConnection` for SQLite (or you may change `Program.cs` to use a different database).
        - JWT settings including: `PrivateKey`, `LifetimeInMinutes`, `Issuer`, and `Audience`.
        - `FrontendAddresses` as a string array.

    - For the **Client project**, you need an `appsettings.json` file that specifies the base address of the API with the key `ApiBaseAddress`.

3. **Restore and build:**
   ```sh
   dotnet restore
   dotnet build
   ```

4. **Run the server:**
   ```sh
   cd Api
   dotnet run
   ```

5. **Run the client:**
   ```sh
   cd Client
   dotnet run
   ```

6. **Run tests:**
   ```sh
   dotnet test
   ```

---

## Security Notice

This project is not production grade and the cryptography has not been audited or thoroughly tested. Please do not use it for storing sensitive data.

---

## License

This project is provided under the MIT License.  
See [LICENSE](LICENSE) for details.