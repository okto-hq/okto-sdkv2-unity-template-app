# Okto Unity SDK Integration Guide

This guide will help you integrate the **Okto Unity SDK** into your Unity project for login, wallet management, and onramp features.  

## 🚀 Getting Started

### 1. **Import the SDK**
- Download and import the `OktoUnitySDK` package into your Unity project.

### 2. **Setup Onboarding (Login)**
1. In your relevant Unity scene:
    - Drag and drop the `OktoHolder` prefab anywhere in the scene.
    - Drag and drop the `OnboardingPrefab` onto the **relevant Canvas**.
2. Assign a button to trigger login:
    - Drag your desired `Login Button` into the public slot of `onBoardingBtn` on the `OnboardingPrefab` component.
3. Drag and drop the **`Authenticate`** prefab into the scene where user authentication should occur.
4. Locate the **`OnboardingTheme`** ScriptableObject in the **Assets** folder. Here, you can customize the onboarding experience by setting your company logo, name, and preferred theme.
5. Locate the **`EnvironmentConfig`** ScriptableObject and configure it with your client details for either the **Sandbox** or **Production** environment, depending on your intended usage.

### 3. **Test**
- Run the scene in the Unity Editor.
- Onboard urself into Okto chain.
- Build for **Android** or **iOS**.

### 4. **Platform Guidelines**
> ⚠️ Make sure to follow respective platform guidelines when building for Android or iOS:
- **Android**: https://docs.okto.tech/docs/unity-sdk/platform-support/android-support
- **iOS**: https://docs.okto.tech/docs/unity-sdk/platform-support/ios-support

---

## 💸 Onramp Setup

### 1. Drag & Drop
- Search and drag the `OnRampHolder` prefab into the **relevant Canvas**.

### 2. Customize OnRamp UI
- Inside the `OnRampHolder`, navigate to `onRamp[Webview]` → `OnRampUiManager`.
- Replace the default `TokenClickBtn` by dragging your own button into the exposed field in the inspector.

---

## 🌐 Supported Networks

### ✅ Mainnets
- **ARBITRUM** (`ARB`)
- **AVALANCHE** (`AVAX`)
- **BASE** (`BASE`)
- **BSC** (`BSC`)
- **ETHEREUM** (`ETH`)
- **GNOSIS** (`GNO`)
- **OPTIMISM** (`OP`)
- **POLYGON** (`POLYGON`)
- **ZKEVM** (`ZKEVM`)

### 🧪 Testnets
- **BASE_TESTNET**
- **HYPERLIQUID_EVM_TESTNET**
- **POLYGON_TESTNET_AMOY**

---