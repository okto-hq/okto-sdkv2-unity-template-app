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
    public class DecodingTest : MonoBehaviour
    {
        [SerializeField] private Decoding callDataDecoder;
        
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
            DecodingCore.DecodedCallData result;
            List<string> testResults = new List<string>();
            int totalTests = 0;
            int passedTests = 0;
            
            void AddTestResult(string testName, DecodingCore.DecodedCallData decodedData, bool expectSuccess)
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
            }
            
            // Decoding tests for all encoded values from AbiEncodingTests
            result = TestDepositDecoding();
            AddTestResult("Deposit Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestDepositWithMetaTxDecoding();
            AddTestResult("Deposit With MetaTx Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestWithdrawToUserDecoding();
            AddTestResult("Withdraw To User Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestWithdrawWithSignatureDecoding();
            AddTestResult("Withdraw With Signature Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestCollectTournamentFeeDecoding();
            AddTestResult("Collect Tournament Fee Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestWithdrawTournamentFeeDecoding();
            AddTestResult("Withdraw Tournament Fee Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestSetTrustedSignerDecoding();
            AddTestResult("Set Trusted Signer Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestSetMultiSigOwnersDecoding();
            AddTestResult("Set MultiSig Owners Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestEmergencyWithdrawDecoding();
            AddTestResult("Emergency Withdraw Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestPauseDecoding();
            AddTestResult("Pause Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestUnpauseDecoding();
            AddTestResult("Unpause Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestTransferERC20Decoding();
            AddTestResult("Transfer ERC20 Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestApproveERC20Decoding();
            AddTestResult("Approve ERC20 Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestTransferFromERC20Decoding();
            AddTestResult("TransferFrom ERC20 Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestPermitDecoding();
            AddTestResult("Permit Decoding", result, true);
            yield return new WaitForSeconds(0.1f);

            result = TestUnsupportedTypeDecoding();
            AddTestResult("Unsupported Types Decoding", result, false);
            yield return new WaitForSeconds(0.1f);
            
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
        
        public DecodingCore.DecodedCallData TestDepositDecoding()
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
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestDepositWithMetaTxDecoding()
        {
            string callData = "0xb4485ffb00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a76400000000000000000000000000000000000000000000000000000000000067748580000000000000000000000000000000000000000000000000000000000000001b12345678901234567890123456789012345678901234567890123456789012341234567890123456789012345678901234567890123456789012345678901234";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""user"", ""type"": ""address""},
                    {""name"": ""amount"", ""type"": ""uint256""},
                    {""name"": ""deadline"", ""type"": ""uint256""},
                    {""name"": ""v"", ""type"": ""uint8""},
                    {""name"": ""r"", ""type"": ""bytes32""},
                    {""name"": ""s"", ""type"": ""bytes32""}
                ],
                ""name"": ""depositWithMetaTx"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestWithdrawToUserDecoding()
        {
            string callData = "0x2b37129500000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""recipient"", ""type"": ""address""},
                    {""name"": ""amount"", ""type"": ""uint256""}
                ],
                ""name"": ""withdrawToUser"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestWithdrawWithSignatureDecoding()
        {
            string callData = "0xc316984600000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000c00000000000000000000000000000000000000000000000000000000067748580000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000125769746864726177616c20726571756573740000000000000000000000000000000000000000000000000000000000000000000000000000000000000000004112345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789000000000000000000000000000000000000000000000000000000000000000";
            
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
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestCollectTournamentFeeDecoding()
        {
            string callData = "0x6d6255c70000000000000000000000000000000000000000000000000de0b6b3a7640000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""amount"", ""type"": ""uint256""}
                ],
                ""name"": ""collectTournamentFee"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestWithdrawTournamentFeeDecoding()
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
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestSetTrustedSignerDecoding()
        {
            string callData = "0x56a1c7010000000000000000000000000000000000000000000000000000000000000001";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""_trustedSigner"", ""type"": ""address""}
                ],
                ""name"": ""setTrustedSigner"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestSetMultiSigOwnersDecoding()
        {
            string callData = "0x21d94df900000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000002";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""owners"", ""type"": ""address[]""},
                    {""name"": ""_minApprovals"", ""type"": ""uint256""}
                ],
                ""name"": ""setMultiSigOwners"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestEmergencyWithdrawDecoding()
        {
            string callData = "0xa288168a00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a764000000000000000000000000000000000000000000000000000000000000000000600000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000800000000000000000000000000000000000000000000000000000000000000002123400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000025678000000000000000000000000000000000000000000000000000000000000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""to"", ""type"": ""address""},
                    {""name"": ""amount"", ""type"": ""uint256""},
                    {""name"": ""signatures"", ""type"": ""bytes[]""}
                ],
                ""name"": ""emergencyWithdraw"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestPauseDecoding()
        {
            string callData = "0x8456cb59";
            
            string abi = @"[{
                ""inputs"": [],
                ""name"": ""pause"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestUnpauseDecoding()
        {
            string callData = "0x3f4ba83a";
            
            string abi = @"[{
                ""inputs"": [],
                ""name"": ""unpause"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestTransferERC20Decoding()
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
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestApproveERC20Decoding()
        {
            string callData = "0x095ea7b300000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""spender"", ""type"": ""address""},
                    {""name"": ""value"", ""type"": ""uint256""}
                ],
                ""name"": ""approve"",
                ""outputs"": [{""name"": """", ""type"": ""bool""}],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestTransferFromERC20Decoding()
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
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestPermitDecoding()
        {
            string callData = "0xd505accf000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000de0b6b3a76400000000000000000000000000000000000000000000000000000000000067748580000000000000000000000000000000000000000000000000000000000000001b12345678901234567890123456789012345678901234567890123456789012341234567890123456789012345678901234567890123456789012345678901234";
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""owner"", ""type"": ""address""},
                    {""name"": ""spender"", ""type"": ""address""},
                    {""name"": ""value"", ""type"": ""uint256""},
                    {""name"": ""deadline"", ""type"": ""uint256""},
                    {""name"": ""v"", ""type"": ""uint8""},
                    {""name"": ""r"", ""type"": ""bytes32""},
                    {""name"": ""s"", ""type"": ""bytes32""}
                ],
                ""name"": ""permit"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
        
        public DecodingCore.DecodedCallData TestUnsupportedTypeDecoding()
        {
            // This should fail as the encoding test also returns an error
            string callData = "0x1234567890abcdef"; // Invalid/fake data
            
            string abi = @"[{
                ""inputs"": [
                    {""name"": ""tupleParam"", ""type"": ""tuple""},
                    {""name"": ""fixedArray"", ""type"": ""uint256[4]""},
                    {""name"": ""customEnum"", ""type"": ""enum""},
                    {""name"": ""nestedTuple"", ""type"": ""tuple[]""},
                    {""name"": ""fixed128x18"", ""type"": ""fixed128x18""}
                ],
                ""name"": ""unsupportedTypes"",
                ""outputs"": [],
                ""stateMutability"": ""nonpayable"",
                ""type"": ""function""
            }]";
            
            return DecodingCore.DecodeCallDataDirectWrapper(callData, abi);
        }
    }
} 