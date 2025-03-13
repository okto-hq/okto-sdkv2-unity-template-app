# Okto SDK Unity Template

This is a Unity template pre-configured with [Okto SDK](https://docs.okto.tech/) for building chain abstracted decentralized applications in Unity. It provides a solid foundation for creating Web3-enabled games and applications with best practices and essential tooling.

## Features

- 🎮 **Unity Integration** with Okto SDK
- 🔐 **Web3 Functionality** for blockchain interactions
- 👤 **Authentication** with Google Sign-In
- 💼 **Wallet Management** for crypto assets
- 🔄 **Blockchain Interactions** made simple

## Prerequisites

Before you begin, ensure you have the following installed:
- Unity 2021.3 LTS or later
- **Okto API Keys:** `clientPrivateKey` and `clientSwa`. Obtain these from the [Okto Developer Dashboard](https://dashboard.okto.tech/login).
- Google OAuth Credentials for authentication

## Required Dependencies

The template requires the following dependencies:
- [Google Sign-In for Unity](https://github.com/googlesamples/google-signin-unity/releases)
- [Play Services Resolver](https://github.com/googlesamples/unity-jar-resolver)
- Newtonsoft.Json (Unity Package: "com.unity.nuget.newtonsoft-json": "3.0.2")

## Getting Started

1. Clone this template:
   ```bash
   git clone https://github.com/okto-hq/okto-sdkv2-unity-template-app.git
   cd okto-sdkv2-unity-template-app
   ```

2. Open the project in Unity.

3. Install the required dependencies:
   - Import the Google Sign-In for Unity package
   - Import the Play Services Resolver package
   - Add Newtonsoft.Json via the Unity Package Manager

4. Set up your credentials:
   - In `Login.cs`, set your Google Web OAuth client ID:
     ```csharp
     webClientId = "YOUR_GOOGLE_WEB_OAUTH_CLIENT_ID"
     ```
   
   - In `OktoClient.cs`, set your Okto API credentials:
     ```csharp
     clientPrivateKey = "YOUR_CLIENT_PRIVATE_KEY"
     clientSwa = "YOUR_CLIENT_SWA"
     ```

5. Run the project in the Unity Editor to test the functionality.

## Demo

You can download and try the latest Android APK demo from the [GitHub repository](https://github.com/okto-hq/okto-sdkv2-unity-template-app/tree/main/Android_Build_Latest).

## Learn More

- [Okto SDK Documentation](https://docs.okto.tech/)
- [Unity Documentation](https://docs.unity3d.com/)
- [Google Console Setup Guide](https://docsv2.okto.tech/docs/unity-sdk/authenticate-users/google-oauth/google-console-setup)

## Contributing

Contributions are welcome! Please take a moment to review our [CONTRIBUTING.md](CONTRIBUTING.md) guidelines before submitting any Pull Requests. Your contributions are invaluable to the Okto community.

## Support

For questions and support:
- Join our [Discord Server](https://discord.com/invite/okto-916349620383252511)
- Visit the [Okto Documentation](https://docs.okto.tech/) 