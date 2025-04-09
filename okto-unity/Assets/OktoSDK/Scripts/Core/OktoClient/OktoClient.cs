using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using Nethereum.Signer;

/*
 * OktoClient Class
 *
 * This class manages authentication and session handling for the Okto SDK. 
 * It is responsible for generating authentication payloads, handling OAuth login, 
 * generating bearer tokens, and managing session states.
 *
 * Features:
 * - Handles user authentication via `LoginUsingOAuth`.
 * - Generates authorization tokens using `GetAuthorizationToken`.
 * - Signs messages securely with Ethereum private keys.
 * - Maintains session configuration and provides utility functions for session validation.
 * - Ensures secure communication with the Okto authentication gateway.
 */
namespace OktoSDK
{
    public class OktoClient
    {
        private ClientConfig _clientConfig;
        public SessionConfig _sessionConfig;
        public UserDetails _userDetails;
        private EnvConfig _envConfig;
        private bool _isDev = true;
        private string idToken;

        public OktoClient(OktoClientConfig config)
        {
            ValidateConfig(config);

            _clientConfig = new ClientConfig
            {
                ClientPrivKey = config.ClientPrivateKey,
                ClientPubKey = GetPublicKey(config.ClientPrivateKey),
                ClientSWA = config.ClientSWA
            };

            _envConfig = config.Environment == OktoEnv.SANDBOX
                ? Constants.SandboxEnvConfig
                : Constants.ProductionEnvConfig;
        }

        //autologin
        public OktoClient(OktoClientConfig config,SessionConfig sessionConfig, UserDetails userDetails)
        {
            ValidateConfig(config);

            _clientConfig = new ClientConfig
            {
                ClientPrivKey = config.ClientPrivateKey,
                ClientPubKey = GetPublicKey(config.ClientPrivateKey),
                ClientSWA = config.ClientSWA
            };

            _sessionConfig = new SessionConfig
            {
                SessionPrivKey = sessionConfig.SessionPrivKey,
                SessionPubKey = sessionConfig.SessionPubKey,
                UserSWA = sessionConfig.UserSWA
            };

            _userDetails = new UserDetails
            {
                env = userDetails.env,
                whatsapp = userDetails.whatsapp, 
                sessionData = userDetails.sessionData
            };

            _envConfig = config.Environment == OktoEnv.SANDBOX
            ? Constants.SandboxEnvConfig
            : Constants.ProductionEnvConfig;
        }

        public async Task<AuthenticateResult> LoginUsingOAuth(AuthData authData, Action<SessionConfig> onSuccess = null)
        {
            ValidateAuthData(authData);

            var clientPrivateKey = _clientConfig.ClientPrivKey;
            var clientSWA = _clientConfig.ClientSWA;
            var session = SessionKey.Create();

            if (string.IsNullOrEmpty(clientPrivateKey) || string.IsNullOrEmpty(clientSWA))
            {
                throw new Exception("Client details not found");
            }

            var authPayload = await GenerateAuthenticatePayload(
                authData,
                session,
                clientSWA,
                clientPrivateKey
            );

            try
            {
                var authRes = await GatewayClientRepository.Authenticate(this, authPayload);

                _sessionConfig = new SessionConfig
                {
                    SessionPrivKey = session.PrivateKeyHexWith0x,
                    SessionPubKey = session.UncompressedPublicKeyHexWith0x,
                    UserSWA = authRes.UserSWA
                };

                onSuccess?.Invoke(_sessionConfig);
                idToken = authData.IdToken;
                return authRes;
            }
            catch (RpcError error)
            {
                RpcErrors rpcError =  new RpcErrors(error.jsonrpc, error.id, error.error);
                string userOrpcErrorpStr = JsonConvert.SerializeObject(rpcError, Formatting.Indented);
                ResponsePanel.SetResponse($"Authentication error: {userOrpcErrorpStr}");
                CustomLogger.Log($"Authentication error: {userOrpcErrorpStr}");
                return null;
            }
        }

        public async Task<string> GetAuthorizationToken()
        {
            //return idToken;
            if (_sessionConfig?.SessionPrivKey == null)
            {
                CustomLogger.LogError("SessionPrivKey is null");
                return null;

            }
            if (_sessionConfig?.SessionPubKey == null)
            {
                CustomLogger.LogError("SessionPubKey is null");
                return null;
            }
            CustomLogger.Log($"_sessionConfig?.SessionPubKey: {_sessionConfig.SessionPubKey}");
            CustomLogger.Log($"_sessionConfig?.SessionPrivKey: {_sessionConfig.SessionPrivKey}");

            var expire_at = DateTimeOffset.UtcNow.AddMinutes(90).ToUnixTimeSeconds(); 

            try
            {
                // Create the data object with the correct format
                var data = new
                {
                    expire_at = expire_at,
                    session_pub_key = _sessionConfig.SessionPubKey
                };

                CustomLogger.Log($"expire_at: {expire_at}");


                string messageToSign = JsonConvert.SerializeObject(data, new JsonSerializerSettings
                {
                    Formatting = Formatting.None,  // Ensure minified JSON (no spaces/indentation)
                    StringEscapeHandling = StringEscapeHandling.EscapeNonAscii  // Ensure escape characters match JS behavior
                });
                CustomLogger.Log($"Message to sign: {messageToSign}");

                string signature = SignMessage(messageToSign, _sessionConfig.SessionPrivKey);
                CustomLogger.Log($"Generated signature: {signature}");

                // Create the token payload with the correct format
                var tokenPayload = new
                {
                    type = "ecdsa_uncompressed",
                    data = data,
                    data_signature = signature
                };

                // Convert to JSON with camelCase formatting
                string jsonPayload = JsonConvert.SerializeObject(tokenPayload, new JsonSerializerSettings
                {
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.None
                });

                CustomLogger.Log($"Token payload before encoding: {jsonPayload}");

                // Convert to Base64
                string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonPayload));
                CustomLogger.Log($"Final token: {token}");

                return token;
            }
            catch (Exception ex)
            {
                CustomLogger.LogError($"Error generating authorization token: {ex.Message}\nStack trace: {ex.StackTrace}");
                throw;
            }
        }

        public static string SignMessage(string message, string privateKey)
        {
            // Convert message to a byte array
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // Convert private key to a hexadecimal string
            string privateKeyHex = privateKey.StartsWith("0x") ? privateKey.Substring(2) : privateKey;

            var signer = new EthereumMessageSigner();
            var signature = signer.Sign(messageBytes, privateKeyHex);

            return signature.StartsWith("0x") ? signature : "0x" + signature;
        }

        public bool IsLoggedIn()
        {
            return _sessionConfig != null;
        }

        public void SessionClear()
        {
            PlayerPrefsManager.Delete();
            _sessionConfig = null;
        }

        public EnvConfig Env => _envConfig;
        public string UserSWA => _sessionConfig?.UserSWA;
        public string ClientSWA => _clientConfig.ClientSWA;

        private void ValidateConfig(OktoClientConfig config)
        {
            if (string.IsNullOrEmpty(config.ClientPrivateKey))
                throw new ArgumentException("ClientPrivateKey is required");
            if (string.IsNullOrEmpty(config.ClientSWA))
                throw new ArgumentException("ClientSWA is required");
            if (string.IsNullOrEmpty(config.Environment))
                throw new ArgumentException("Environment is required");
        }

        private void ValidateAuthData(AuthData authData)
        {
            if (string.IsNullOrEmpty(authData.IdToken))
                throw new ArgumentException("IdToken is required");
            if (string.IsNullOrEmpty(authData.Provider))
                throw new ArgumentException("Provider is required");
        }

        private string GetPublicKey(string privateKey)
        {
            var privateKeyHex = privateKey.RemoveHexPrefix();
            var key = new EthECKey(privateKeyHex);
            return "0x" + key.GetPubKey().ToHex();
        }

        private async Task<AuthenticatePayloadParam> GenerateAuthenticatePayload(
            AuthData authData,
            SessionKey session,
            string clientSWA,
            string clientPrivateKey)
        {
            return await AuthenticationPayload.Generate(
                this,
                authData,
                session,
                clientSWA,
                clientPrivateKey
            );
        }
    }
}