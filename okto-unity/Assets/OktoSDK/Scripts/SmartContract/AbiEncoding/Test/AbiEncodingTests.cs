using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace OktoSDK.Features.SmartContract
{
    public class AbiEncodingTests : MonoBehaviour
    {

        private void OnEnable()
        {
            StartCoroutine(RunTests());
        }

        private IEnumerator RunTests()
        {
            string result;
            string expected;
            List<string> testResults = new List<string>();
            int totalTests = 0;
            int passedTests = 0;

            void AddTestResult(string testName, string result, string expected)
            {
                totalTests++;
                bool isMatch = result.Equals(expected, StringComparison.OrdinalIgnoreCase);
                if (isMatch) passedTests++;

                string testResult = $"{testName}:\nResult: {result}\nExpected: {expected}\nMatch: {isMatch}\n";
                testResults.Add(testResult);
                CustomLogger.Log($"Test: {testName}\nResult: {result}\nExpected: {expected}\nMatch: {isMatch}");

                ResponsePanel.SetResponse($"{testResult}\n\nProgress: {passedTests}/{totalTests} tests passed");
            }
            // Run all non-view tests
            result = TestDepositEncoding();
            expected = "0xb6b55f250000000000000000000000000000000000000000000000000de0b6b3a7640000"; // ethers.js generated value
            AddTestResult("Deposit Test", result, expected);

            result = TestDepositWithMetaTxEncoding();
            expected = "0xb4485ffb00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a76400000000000000000000000000000000000000000000000000000000000067748580000000000000000000000000000000000000000000000000000000000000001b12345678901234567890123456789012345678901234567890123456789012341234567890123456789012345678901234567890123456789012345678901234";
            AddTestResult("Deposit With MetaTx Test", result, expected);

            result = TestWithdrawToUserEncoding();
            expected = "0x2b37129500000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000";
            AddTestResult("Withdraw To User Test", result, expected);

            result = TestWithdrawWithSignatureEncoding();
            expected = "0xc316984600000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000c00000000000000000000000000000000000000000000000000000000067748580000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000000125769746864726177616c207265717565737400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000041123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789000000000000000000000000000000000000000000000000000000000000000";
            AddTestResult("Withdraw With Signature Test", result, expected);

            result = TestCollectTournamentFeeEncoding();
            expected = "0x6d6255c70000000000000000000000000000000000000000000000000de0b6b3a7640000";
            AddTestResult("Collect Tournament Fee Test", result, expected);

            result = TestWithdrawTournamentFeeEncoding();
            expected = "0x8f8a6eee00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a764000000000000000000000000000000000000000000000000000000000000000000600000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000800000000000000000000000000000000000000000000000000000000000000002123400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000025678000000000000000000000000000000000000000000000000000000000000";
            AddTestResult("Withdraw Tournament Fee Test", result, expected);

            result = TestSetTrustedSignerEncoding();
            expected = "0x56a1c7010000000000000000000000000000000000000000000000000000000000000001";
            AddTestResult("Set Trusted Signer Test", result, expected);

            result = TestSetMultiSigOwnersEncoding();
            expected = "0x21d94df900000000000000000000000000000000000000000000000000000000000000400000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000000200000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000000000000000002";
            AddTestResult("Set MultiSig Owners Test", result, expected);

            result = TestEmergencyWithdrawEncoding();
            expected = "0xa288168a00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a764000000000000000000000000000000000000000000000000000000000000000000600000000000000000000000000000000000000000000000000000000000000002000000000000000000000000000000000000000000000000000000000000004000000000000000000000000000000000000000000000000000000000000000800000000000000000000000000000000000000000000000000000000000000002123400000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000025678000000000000000000000000000000000000000000000000000000000000";
            AddTestResult("Emergency Withdraw Test", result, expected);

            result = TestPauseEncoding();
            expected = "0x8456cb59";
            AddTestResult("Pause Test", result, expected);

            result = TestUnpauseEncoding();
            expected = "0x3f4ba83a";
            AddTestResult("Unpause Test", result, expected);

            result = TestTransferERC20Encoding();
            expected = "0xa9059cbb00000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000";
            AddTestResult("Transfer ERC20 Test", result, expected);

            result = TestApproveERC20Encoding();
            expected = "0x095ea7b300000000000000000000000000000000000000000000000000000000000000010000000000000000000000000000000000000000000000000de0b6b3a7640000";
            AddTestResult("Approve ERC20 Test", result, expected);

            result = TestTransferFromERC20Encoding();
            expected = "0x23b872dd000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000de0b6b3a7640000";
            AddTestResult("TransferFrom ERC20 Test", result, expected);

            result = TestPermitEncoding();
            expected = "0xd505accf000000000000000000000000000000000000000000000000000000000000000100000000000000000000000000000000000000000000000000000000000000020000000000000000000000000000000000000000000000000de0b6b3a76400000000000000000000000000000000000000000000000000000000000067748580000000000000000000000000000000000000000000000000000000000000001b12345678901234567890123456789012345678901234567890123456789012341234567890123456789012345678901234567890123456789012345678901234";
            AddTestResult("Permit Test", result, expected);

            result = TestUnsupportedTypeEncoding();
            expected = "Error: Error encoding function data for 'unsupportedTypes': Unsupported parameter type: tuple";
            AddTestResult("Unsupported Types Test", result, expected);

            result = TestUnsupportedOutputTypeEncoding();
            expected = "Error: Error encoding function data for 'getComplexData': Function has unsupported output types: tuple, enum Status, uint256[3]";
            AddTestResult("Unsupported Output Types Test", result, expected);

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
            yield return null;
        }

        public string TestDepositEncoding()
        {
            try
            {
                string abi = @"[{
                    ""inputs"": [
                        {""name"": ""amount"", ""type"": ""uint256""}
                    ],
                    ""name"": ""deposit"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                }]";

                string functionName = "deposit";
                //BigInteger amount = BigInteger.Parse("1000000000000000000");
                object[] parameters = new object[] { "1000000000000000000" };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestDepositWithMetaTxEncoding()
        {
            try
            {
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

                string functionName = "depositWithMetaTx";

                // Prepare parameters with exact values
                string userAddress = "0x0000000000000000000000000000000000000001";
                BigInteger amount = BigInteger.Parse("1000000000000000000");
                BigInteger deadline = BigInteger.Parse("1735689600");
                byte v = 27;
                byte[] r = new byte[32];
                byte[] s = new byte[32];

                string rHex = "1234567890123456789012345678901234567890123456789012345678901234";
                string sHex = "1234567890123456789012345678901234567890123456789012345678901234";

                for (int i = 0; i < 32; i++)
                {
                    r[i] = Convert.ToByte(rHex.Substring(i * 2, 2), 16);
                    s[i] = Convert.ToByte(sHex.Substring(i * 2, 2), 16);
                }

                object[] parameters = new object[] {
                    userAddress,
                    amount,
                    deadline,
                    v,
                    r,
                    s
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestWithdrawToUserEncoding()
        {
            try
            {
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

                string functionName = "withdrawToUser";
                string recipientAddress = "0x0000000000000000000000000000000000000001";
                BigInteger amount = BigInteger.Parse("1000000000000000000");

                object[] parameters = new object[] {
                    recipientAddress,
                    amount
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestWithdrawWithSignatureEncoding()
        {
            try
            {
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

                string functionName = "withdrawWithSignature";
                string recipientAddress = "0x0000000000000000000000000000000000000001";
                BigInteger amount = BigInteger.Parse("1000000000000000000");
                BigInteger nonce = BigInteger.Parse("1");
                string message = "Withdrawal request";
                BigInteger expiry = BigInteger.Parse("1735689600");
                byte[] signature = "0x1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890".HexToByteArray();

                object[] parameters = new object[] {
                    recipientAddress,
                    amount,
                    nonce,
                    message,
                    expiry,
                    signature
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestCollectTournamentFeeEncoding()
        {
            try
            {
                string abi = @"[{
                    ""inputs"": [
                        {""name"": ""amount"", ""type"": ""uint256""}
                    ],
                    ""name"": ""collectTournamentFee"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                }]";

                string functionName = "collectTournamentFee";
                BigInteger amount = BigInteger.Parse("1000000000000000000");

                object[] parameters = new object[] { amount };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestWithdrawTournamentFeeEncoding()
        {
            try
            {
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

                string functionName = "withdrawTournamentFee";
                string toAddress = "0x0000000000000000000000000000000000000001";
                BigInteger amount = BigInteger.Parse("1000000000000000000");
                byte[][] signatures = new byte[][] {
                    "0x1234".HexToByteArray(),
                    "0x5678".HexToByteArray()
                };

                object[] parameters = new object[] {
                    toAddress,
                    amount,
                    signatures
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestSetTrustedSignerEncoding()
        {
            try
            {
                string abi = @"[{
                    ""inputs"": [
                        {""name"": ""_trustedSigner"", ""type"": ""address""}
                    ],
                    ""name"": ""setTrustedSigner"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                }]";

                string functionName = "setTrustedSigner";
                string trustedSigner = "0x0000000000000000000000000000000000000001";

                object[] parameters = new object[] { trustedSigner };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestSetMultiSigOwnersEncoding()
        {
            try
            {
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

                string functionName = "setMultiSigOwners";
                string[] owners = new string[] {
                    "0x0000000000000000000000000000000000000001",
                    "0x0000000000000000000000000000000000000002"
                };
                BigInteger minApprovals = BigInteger.Parse("2");

                object[] parameters = new object[] {
                    owners,
                    minApprovals
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestEmergencyWithdrawEncoding()
        {
            try
            {
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

                string functionName = "emergencyWithdraw";
                string toAddress = "0x0000000000000000000000000000000000000001";
                BigInteger amount = BigInteger.Parse("1000000000000000000");
                byte[][] signatures = new byte[][] {
                    "0x1234".HexToByteArray(),
                    "0x5678".HexToByteArray()
                };

                object[] parameters = new object[] {
                    toAddress,
                    amount,
                    signatures
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestPauseEncoding()
        {
            try
            {
                string abi = @"[{
                    ""inputs"": [],
                    ""name"": ""pause"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                }]";

                string functionName = "pause";
                object[] parameters = new object[] { };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestUnpauseEncoding()
        {
            try
            {
                string abi = @"[{
                    ""inputs"": [],
                    ""name"": ""unpause"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                }]";

                string functionName = "unpause";
                object[] parameters = new object[] { };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestTransferERC20Encoding()
        {
            try
            {
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

                string functionName = "transfer";
                string toAddress = "0x0000000000000000000000000000000000000001";
                BigInteger value = BigInteger.Parse("1000000000000000000");

                object[] parameters = new object[] {
                    toAddress,
                    value
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestApproveERC20Encoding()
        {
            try
            {
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

                string functionName = "approve";

                object[] parameters = new object[] {
                    "0x0000000000000000000000000000000000000001",
                    "1000000000000000000" // 1 ETH in Wei as decimal
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestTransferFromERC20Encoding()
        {
            try
            {
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

                string functionName = "transferFrom";
                string fromAddress = "0x0000000000000000000000000000000000000001";
                string toAddress = "0x0000000000000000000000000000000000000002";
                BigInteger value = BigInteger.Parse("1000000000000000000");

                object[] parameters = new object[] {
                    fromAddress,
                    toAddress,
                    value
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestPermitEncoding()
        {
            try
            {
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

                string functionName = "permit";
                string ownerAddress = "0x0000000000000000000000000000000000000001";
                string spenderAddress = "0x0000000000000000000000000000000000000002";
                BigInteger value = BigInteger.Parse("1000000000000000000");
                BigInteger deadline = BigInteger.Parse("1735689600");
                byte v = 27;
                byte[] r = "0x1234567890123456789012345678901234567890123456789012345678901234".HexToByteArray();
                byte[] s = "0x1234567890123456789012345678901234567890123456789012345678901234".HexToByteArray();

                object[] parameters = new object[] {
                    ownerAddress,
                    spenderAddress,
                    value,
                    deadline,
                    v,
                    r,
                    s
                };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestUnsupportedTypeEncoding()
        {
            try
            {
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

                string functionName = "unsupportedTypes";
                object[] parameters = new object[] {
            "(1,2,3)",  // tuple
            "[1,2,3,4]",  // fixed array
            "2",  // enum
            "[(1,2),(3,4)]",  // nested tuple array
            "12.345"  // fixed point number
        };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public string TestUnsupportedOutputTypeEncoding()
        {
            try
            {
                string abi = @"[{
            ""inputs"": [
                {""name"": ""amount"", ""type"": ""uint256""},
                {""name"": ""recipient"", ""type"": ""address""}
            ],
            ""name"": ""getComplexData"",
            ""outputs"": [
                {""name"": ""result"", ""type"": ""tuple""},
                {""name"": ""status"", ""type"": ""enum Status""},
                {""name"": ""fixedSizeArray"", ""type"": ""uint256[3]""}
            ],
            ""stateMutability"": ""view"",
            ""type"": ""function""
        }]";

                string functionName = "getComplexData";
                object[] parameters = new object[] {
            "1000000000000000000",  // amount (1 ETH in Wei)
            "0x0000000000000000000000000000000000000001"  // valid address
        };

                string encodedData = AbiEncoding.EncodeFunctionData(abi, functionName, parameters);
                return encodedData;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
