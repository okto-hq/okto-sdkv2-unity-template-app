using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using OktoSDK.Features.SmartContract;

namespace OktoSDK
{
    /// <summary>
    /// Test class for CallDataDecoder to demonstrate decoding various function calls
    /// </summary>
    public class CallDataDecoderTests : MonoBehaviour
    {
        [SerializeField] private CallDataDecoder callDataDecoder;
        
        private void OnEnable()
        {
            if (callDataDecoder == null)
            {
                CustomLogger.LogError("CallDataDecoder reference is missing. Please assign it in the inspector.");
                return;
            }
            
            StartCoroutine(RunTests());
        }
        
        private IEnumerator RunTests()
        {
            CallDataDecoder.DecodedCallData result;
            List<string> testResults = new List<string>();
            int totalTests = 0;
            int passedTests = 0;
            
            void AddTestResult(string testName, CallDataDecoder.DecodedCallData decodedData, bool expectSuccess)
            {
                totalTests++;
                bool success = decodedData.IsSuccessful == expectSuccess;
                if (success) passedTests++;
                
                string testResult = $"Test: {testName}\n" +
                                   $"Result: {(decodedData.IsSuccessful ? "Success" : "Failed")}\n" +
                                   $"Expected: {(expectSuccess ? "Success" : "Failed")}\n" +
                                   $"Function: {decodedData.FunctionName}\n";
                
                if (!string.IsNullOrEmpty(decodedData.ErrorMessage))
                {
                    testResult += $"Error: {decodedData.ErrorMessage}\n";
                }
                
                testResult += "Parameters:\n";
                foreach (var param in decodedData.Parameters)
                {
                    testResult += $"  {param.Key}: {param.Value}\n";
                }
                
                testResults.Add(testResult);
                CustomLogger.Log(testResult);
                
                ResponsePanel.SetResponse($"{testResult}\n\nProgress: {passedTests}/{totalTests} tests passed");
                
                // Wait a short time to let the UI update
                // In a real test, you would use coroutines more effectively
            }
            
            // Test 1: Decode a transfer function
            result = TestTransferERC20Decoding();
            AddTestResult("Transfer ERC20 Decoding", result, true);
            yield return new WaitForSeconds(0.2f);
            
            // Test 2: Decode a deposit function
            result = TestDepositDecoding();
            AddTestResult("Deposit Decoding", result, true);
            yield return new WaitForSeconds(0.2f);
            
            // Test 3: Decode a complex withdraw function with signature
            result = TestWithdrawWithSignatureDecoding();
            AddTestResult("Withdraw With Signature Decoding", result, true);
            yield return new WaitForSeconds(0.2f);
            
            // Test 4: Decode an invalid calldata
            result = TestInvalidCalldataDecoding();
            AddTestResult("Invalid Calldata Decoding", result, false);
            yield return new WaitForSeconds(0.2f);
            
            // Test 5: Decode with invalid ABI
            result = TestInvalidAbiDecoding();
            AddTestResult("Invalid ABI Decoding", result, false);
            yield return new WaitForSeconds(0.2f);
            
            // Test 6: Decode multi-parameter function
            result = TestMultiParamFunctionDecoding();
            AddTestResult("Multi-Parameter Function Decoding", result, true);
            yield return new WaitForSeconds(0.2f);
            
            // Test 7: Decode with array parameters
            result = TestArrayParametersDecoding();
            AddTestResult("Array Parameters Decoding", result, true);
            yield return new WaitForSeconds(0.2f);
            
            // Show final summary
            string finalSummary = $"=== Test Summary ===\n" +
                                $"Total Tests: {totalTests}\n" +
                                $"Passed: {passedTests}\n" +
                                $"Failed: {totalTests - passedTests}\n" +
                                $"Success Rate: {(float)passedTests / totalTests * 100:F2}%\n\n" +
                                $"=== Detailed Results ===\n" +
                                string.Join("\n", testResults);
                                
            ResponsePanel.SetResponse(finalSummary);
            CustomLogger.Log(finalSummary);
        }
        
        public CallDataDecoder.DecodedCallData TestTransferERC20Decoding()
        {
            string callData = "0xa9059cbb00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""to"", ""type"": ""address""},
                    {""name"": ""value"", ""type"": ""uint256""}
                ],
                ""name"": ""transfer"",
                ""outputs"": [{""name"": """", ""type"": ""bool""}],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return callDataDecoder.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public CallDataDecoder.DecodedCallData TestDepositDecoding()
        {
            string callData = "0xb6b55f250000000000000000000000000000000000000000000000000de0b6b3a7640000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""amount"", ""type"": ""uint256""}
                ],
                ""name"": ""deposit"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return callDataDecoder.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public CallDataDecoder.DecodedCallData TestWithdrawWithSignatureDecoding()
        {
            string callData = "0xc316984600000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000c00000000000000000000000000000000000000000000000000000000067748580000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000125769746864726177616c207265717565737400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000041123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789000000000000000000000000000000000000000000000000000000000000000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""recipient"", ""type"": ""address""},
                    {""name"": ""amount"", ""type"": ""uint256""},
                    {""name"": ""nonce"", ""type"": ""uint256""},
                    {""name"": ""message"", ""type"": ""string""},
                    {""name"": ""expiry"", ""type"": ""uint256""},
                    {""name"": ""signature"", ""type"": ""bytes""}
                ],
                ""name"": ""withdrawWithSignature"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return callDataDecoder.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public CallDataDecoder.DecodedCallData TestInvalidCalldataDecoding()
        {
            string callData = "0x1234"; // Too short
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""amount"", ""type"": ""uint256""}
                ],
                ""name"": ""deposit"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return callDataDecoder.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public CallDataDecoder.DecodedCallData TestInvalidAbiDecoding()
        {
            string callData = "0xa9059cbb00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""wrongType"", ""type"": ""tuple""},
                    {""name"": ""unsupportedType"", ""type"": ""enum""}
                ],
                ""name"": ""wrongFunction"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return callDataDecoder.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public CallDataDecoder.DecodedCallData TestMultiParamFunctionDecoding()
        {
            string callData = "0x23b872dd000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000de0b6b3a7640000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""from"", ""type"": ""address""},
                    {""name"": ""to"", ""type"": ""address""},
                    {""name"": ""value"", ""type"": ""uint256""}
                ],
                ""name"": ""transferFrom"",
                ""outputs"": [{""name"": """", ""type"": ""bool""}],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return callDataDecoder.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public CallDataDecoder.DecodedCallData TestArrayParametersDecoding()
        {
            string callData = "0x8f8a6eee00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a764000000000000000000000000000000000000000000000000000000000000000000600000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000800000000000000000000000000000000000000000000000000000000000000002123400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000025678000000000000000000000000000000000000000000000000000000000000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""to"", ""type"": ""address""},
                    {""name"": ""amount"", ""type"": ""uint256""},
                    {""name"": ""signatures"", ""type"": ""bytes[]""}
                ],
                ""name"": ""withdrawTournamentFee"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return callDataDecoder.DecodeCallDataDirectWrapper(callData, abi);
        }
    }
} 