using OktoSDK.Auth;
using OktoSDK.Example;
using OktoSDK.Features.Order;
using OktoSDK.Features.SmartContract;
using OktoSDK.Features.Transaction;
using OktoSDK.Features.Wallet;
using UnityEngine;

namespace OktoSDK.Guide
{
    /// <summary>
    /// Okto SDK Setup Guide
    /// This script provides documentation on how to set up the Okto SDK with the new reorganized structure.
    /// It is not meant to be used in production, but rather as a guide for developers.
    /// </summary>
    public class OktoSetupGuide : MonoBehaviour
    {
        /// <summary>
        /// Recommended scene hierarchy setup for Okto SDK
        /// 
        /// [OktoSDK]
        ///     ├── [Authentication] - Contains auth-related components
        ///     │       └── OktoAuthManager
        ///     │
        ///     ├── [BFF] - Contains Backend-for-Frontend related components
        ///     │       └── ApiClientManager
        ///     │
        ///     ├── [Features]
        ///     │       ├── [Wallet] - Contains wallet-related components
        ///     │       │       └── WalletManager
        ///     │       │
        ///     │       ├── [Order] - Contains order-related components
        ///     │       │       └── OrderPrefab
        ///     │       │
        ///     │       ├── [Network] - Contains network-related components
        ///     │       │       └── NetworkInfoManager
        ///     │       │
        ///     │       ├── [Transaction] - Contains transaction-related components
        ///     │       │       ├── PrefabManager
        ///     │       │       ├── ChainManager
        ///     │       │       └── TransactionManager
        ///     │       │
        ///     │       └── [SmartContract] - Contains smart contract interaction components
        ///     │               └── CallDataDecoder
        ///     │
        ///     └── OktoFeatureManager - Main coordination component
        /// </summary>
        [Header("Documentation Only")]
        [TextArea(5, 20)]
        [SerializeField] private string setupInstructions = @"
# Okto SDK Setup Guide

## New Folder Structure
The SDK has been reorganized into the following main folders:

1. Auth - Authentication-related components
2. BFF - Backend-for-Frontend API clients and data models
3. Features - Feature-specific components, organized by domain:
   * Wallet - Wallet management
   * Order - Order handling
   * Network - Network information and management
   * Transaction - Transaction execution
   * SmartContract - Smart contract interaction

## Scene Setup Instructions

1. Create the main GameObject structure:
   * Create an empty GameObject named 'OktoSDK'
   * Add child GameObjects for each major section:
     - Authentication
     - BFF
     - Features
   * Under Features, add child GameObjects for each feature domain:
     - Wallet
     - Order
     - Network
     - Transaction
     - SmartContract

2. Add core components:
   * Add OktoAuthManager to the Authentication GameObject
   * Add WalletManager to the Wallet GameObject
   * Add OrderPrefab to the Order GameObject
   * Add ChainManager and TransactionManager to the Transaction GameObject
   * Add CallDataDecoder to the SmartContract GameObject
   * Add PrefabManager to the Transaction GameObject
   * Add OktoFeatureManager to the main OktoSDK GameObject

3. Configure component references:
   * In PrefabManager, assign:
     - ChainManager
     - WalletManager
     - TransactionManager
     - CallDataDecoder
   * In OktoFeatureManager, assign:
     - Wallet Feature Container (GameObject)
     - Order Feature Container (GameObject)
     - Transaction Feature Container (GameObject)
     - Smart Contract Feature Container (GameObject)
     - Network Feature Container (GameObject)

## Important Usage Notes

1. Authentication should always be initialized first
2. The OktoFeatureManager will automatically try to find all components
3. Always check if the user is authenticated before using feature-specific functionality
4. Handle events from the various managers to respond to state changes
";

        /// <summary>
        /// Script references that need to be updated for proper imports
        /// </summary>
        [Header("Script References")]
        [TextArea(5, 10)]
        [SerializeField] private string scriptReferences = @"
# Script References

## Auth Namespace
- OktoAuthManager.cs
- GoogleAuthManager.cs
- JwtAuthenticate.cs

## BFF Namespace
- ApiClient.cs
- BffClientRepository.cs
- DataModels.cs
- TransactionDataExtractor.cs

## Features Namespace

### Wallet
- WalletManager.cs
- WalletService.cs
- AccountPrefab.cs

### Order
- OrderPrefab.cs
- OrderService.cs
- OrderFilter.cs
- OrderFilterUI.cs

### Network
- NetworkService.cs

### Transaction
- TransactionManager.cs
- ChainManager.cs
- PrefabManager.cs
- UserOp/*.cs (All files in UserOp folder)

### SmartContract
- CallDataDecoder.cs
- ReadSmartContract/*.cs (All files in ReadSmartContract folder)
- AbiEncoding/*.cs (All files in AbiEncoding folder)
";

        /// <summary>
        /// This method is for documentation only and is not meant to be used at runtime
        /// </summary>
        private void SetupOktoSDK()
        {
            // This is a hypothetical example of setting up the SDK programmatically
            // In practice, you would typically set up the SDK through the Unity Editor

            // 1. Create the main GameObject structure
            var oktoSDK = new GameObject("OktoSDK");
            
            var authObj = new GameObject("Authentication");
            authObj.transform.SetParent(oktoSDK.transform);
            
            var bffObj = new GameObject("BFF");
            bffObj.transform.SetParent(oktoSDK.transform);
            
            var featuresObj = new GameObject("Features");
            featuresObj.transform.SetParent(oktoSDK.transform);
            
            var walletObj = new GameObject("Wallet");
            walletObj.transform.SetParent(featuresObj.transform);
            
            var orderObj = new GameObject("Order");
            orderObj.transform.SetParent(featuresObj.transform);
            
            var networkObj = new GameObject("Network");
            networkObj.transform.SetParent(featuresObj.transform);
            
            var transactionObj = new GameObject("Transaction");
            transactionObj.transform.SetParent(featuresObj.transform);
            
            var smartContractObj = new GameObject("SmartContract");
            smartContractObj.transform.SetParent(featuresObj.transform);
            
            // 2. Add components
            var authManager = authObj.AddComponent<OktoAuthManager>();
            var walletManager = walletObj.AddComponent<WalletManager>();
            var orderPrefab = orderObj.AddComponent<OrderPrefab>();
            var chainManager = transactionObj.AddComponent<ChainManager>();
            var transactionManager = transactionObj.AddComponent<TransactionManager>();
            var callDataDecoder = smartContractObj.AddComponent<Decoding>();
            var prefabManager = transactionObj.AddComponent<PrefabManager>();
            
            // 3. Add the main feature manager
            //var featureManager = oktoSDK.AddComponent<OktoFeatureManager>();
            
            // 4. Configure references
            // Set up references between components
            // Note: In real usage, you would do this through the Unity Editor Inspector
            //prefabManager.SetupReferences(chainManager, walletManager, transactionManager, callDataDecoder);
            
            // 5. Set feature container references on the feature manager
            // This would typically be done in the Unity Editor Inspector
            /*
            featureManager.SetFeatureContainers(
                walletObj,
                orderObj,
                transactionObj,
                smartContractObj,
                networkObj
            );
            */
        }
    }
} 