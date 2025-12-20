using Blaze3SDK.Blaze.Authentication;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class AuthenticationComponentBase
    {
        public const ushort Id = 1;
        public const string Name = "AuthenticationComponent";

        public class Server : BlazeServerComponent<AuthenticationComponentCommand, AuthenticationComponentNotification, Blaze3RpcError>
        {
            public Server() : base(AuthenticationComponentBase.Id, AuthenticationComponentBase.Name)
            {

            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.createAccount)]
            public virtual Task<CreateAccountResponse> CreateAccountAsync(CreateAccountParameters request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.updateAccount)]
            public virtual Task<UpdateAccountResponse> UpdateAccountAsync(UpdateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.updateParentalEmail)]
            public virtual Task<NullStruct> UpdateParentalEmailAsync(UpdateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listUserEntitlements2)]
            public virtual Task<Entitlements> ListUserEntitlements2Async(ListUserEntitlements2Request request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getAccount)]
            public virtual Task<AccountInfo> GetAccountAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.grantEntitlement)]
            public virtual Task<NullStruct> GrantEntitlementAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listEntitlements)]
            public virtual Task<Entitlements> ListEntitlementsAsync(ListEntitlementsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.hasEntitlement)]
            public virtual Task<NullStruct> HasEntitlementAsync(HasEntitlementRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getUseCount)]
            public virtual Task<UseCount> GetUseCountAsync(GetUseCountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.decrementUseCount)]
            public virtual Task<DecrementUseCount> DecrementUseCountAsync(DecrementUseCountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getAuthToken)]
            public virtual Task<GetAuthTokenResponse> GetAuthTokenAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getHandoffToken)]
            public virtual Task<GetHandoffTokenResponse> GetHandoffTokenAsync(GetHandoffTokenRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getPasswordRules)]
            public virtual Task<PasswordRulesInfo> GetPasswordRulesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.grantEntitlement2)]
            public virtual Task<GrantEntitlement2Response> GrantEntitlement2Async(GrantEntitlement2Request request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.login)]
            public virtual Task<LoginResponse> LoginAsync(LoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.acceptTos)]
            public virtual Task<NullStruct> AcceptTosAsync(AcceptTosRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getTosInfo)]
            public virtual Task<GetTosInfoResponse> GetTosInfoAsync(GetTosInfoRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.modifyEntitlement2)]
            public virtual Task<NullStruct> ModifyEntitlement2Async(ModifyEntitlement2Request request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.consumecode)]
            public virtual Task<ConsumecodeResponse> ConsumecodeAsync(ConsumecodeRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.passwordForgot)]
            public virtual Task<NullStruct> PasswordForgotAsync(PasswordForgotRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getTermsAndConditionsContent)]
            public virtual Task<NullStruct> GetTermsAndConditionsContentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getPrivacyPolicyContent)]
            public virtual Task<GetLegalDocContentResponse> GetPrivacyPolicyContentAsync(GetLegalDocContentRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listPersonaEntitlements2)]
            public virtual Task<NullStruct> ListPersonaEntitlements2Async(ListPersonaEntitlements2Request request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.silentLogin)]
            public virtual Task<FullLoginResponse> SilentLoginAsync(SilentLoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.checkAgeReq)]
            public virtual Task<CheckAgeReqResponse> CheckAgeReqAsync(CheckAgeReqRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getOptIn)]
            public virtual Task<OptInValue> GetOptInAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.enableOptIn)]
            public virtual Task<NullStruct> EnableOptInAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.disableOptIn)]
            public virtual Task<NullStruct> DisableOptInAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.expressLogin)]
            public virtual Task<FullLoginResponse> ExpressLoginAsync(ExpressLoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.logout)]
            public virtual Task<NullStruct> LogoutAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.createPersona)]
            public virtual Task<PersonaDetails> CreatePersonaAsync(CreatePersonaRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getPersona)]
            public virtual Task<GetPersonaResponse> GetPersonaAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listPersonas)]
            public virtual Task<ListPersonasResponse> ListPersonasAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.loginPersona)]
            public virtual Task<SessionInfo> LoginPersonaAsync(LoginPersonaRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.logoutPersona)]
            public virtual Task<NullStruct> LogoutPersonaAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.deletePersona)]
            public virtual Task<NullStruct> DeletePersonaAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.disablePersona)]
            public virtual Task<NullStruct> DisablePersonaAsync(PersonaRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listDeviceAccounts)]
            public virtual Task<ListDeviceAccountsResponse> ListDeviceAccountsAsync(ListDeviceAccountsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxCreateAccount)]
            public virtual Task<ConsoleCreateAccountResponse> XboxCreateAccountAsync(ConsoleCreateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.originLogin)]
            public virtual Task<FullLoginResponse> OriginLoginAsync(OriginLoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxAssociateAccount)]
            public virtual Task<NullStruct> XboxAssociateAccountAsync(ConsoleAssociateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxLogin)]
            public virtual Task<ConsoleLoginResponse> XboxLoginAsync(XboxLoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3CreateAccount)]
            public virtual Task<ConsoleCreateAccountResponse> Ps3CreateAccountAsync(ConsoleCreateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3AssociateAccount)]
            public virtual Task<NullStruct> Ps3AssociateAccountAsync(ConsoleAssociateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3Login)]
            public virtual Task<ConsoleLoginResponse> Ps3LoginAsync(PS3LoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.validateSessionKey)]
            public virtual Task<NullStruct> ValidateSessionKeyAsync(ValidateSessionKeyRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.createWalUserSession)]
            public virtual Task<NullStruct> CreateWalUserSessionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.acceptLegalDocs)]
            public virtual Task<NullStruct> AcceptLegalDocsAsync(AcceptLegalDocsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getLegalDocsInfo)]
            public virtual Task<GetLegalDocsInfoResponse> GetLegalDocsInfoAsync(GetLegalDocsInfoRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getTermsOfServiceContent)]
            public virtual Task<GetLegalDocContentResponse> GetTermsOfServiceContentAsync(GetLegalDocContentRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.deviceLoginGuest)]
            public virtual Task<ConsoleLoginResponse> DeviceLoginGuestAsync(DeviceLoginGuestRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.checkSinglePlayerLogin)]
            public virtual Task<NullStruct> CheckSinglePlayerLoginAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }


            public override Type GetCommandRequestType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AuthenticationComponentNotification notification) => AuthenticationComponentBase.GetNotificationType(notification);

        }

        public class Client : BlazeClientComponent<AuthenticationComponentCommand, AuthenticationComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }

            public Client(BlazeClientConnection connection) : base(AuthenticationComponentBase.Id, AuthenticationComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }


            public CreateAccountResponse CreateAccount(CreateAccountParameters request)
            {
                return Connection.SendRequest<CreateAccountParameters, CreateAccountResponse, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.createAccount, request);
            }
            public Task<CreateAccountResponse> CreateAccountAsync(CreateAccountParameters request)
            {
                return Connection.SendRequestAsync<CreateAccountParameters, CreateAccountResponse, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.createAccount, request);
            }

            public UpdateAccountResponse UpdateAccount(UpdateAccountRequest request)
            {
                return Connection.SendRequest<UpdateAccountRequest, UpdateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateAccount, request);
            }
            public Task<UpdateAccountResponse> UpdateAccountAsync(UpdateAccountRequest request)
            {
                return Connection.SendRequestAsync<UpdateAccountRequest, UpdateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateAccount, request);
            }

            public NullStruct UpdateParentalEmail(UpdateAccountRequest request)
            {
                return Connection.SendRequest<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateParentalEmail, request);
            }
            public Task<NullStruct> UpdateParentalEmailAsync(UpdateAccountRequest request)
            {
                return Connection.SendRequestAsync<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateParentalEmail, request);
            }

            public Entitlements ListUserEntitlements2(ListUserEntitlements2Request request)
            {
                return Connection.SendRequest<ListUserEntitlements2Request, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.listUserEntitlements2, request);
            }
            public Task<Entitlements> ListUserEntitlements2Async(ListUserEntitlements2Request request)
            {
                return Connection.SendRequestAsync<ListUserEntitlements2Request, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.listUserEntitlements2, request);
            }

            public AccountInfo GetAccount()
            {
                return Connection.SendRequest<NullStruct, AccountInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAccount, new NullStruct());
            }
            public Task<AccountInfo> GetAccountAsync()
            {
                return Connection.SendRequestAsync<NullStruct, AccountInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAccount, new NullStruct());
            }

            public NullStruct GrantEntitlement()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement, new NullStruct());
            }
            public Task<NullStruct> GrantEntitlementAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement, new NullStruct());
            }

            public Entitlements ListEntitlements(ListEntitlementsRequest request)
            {
                return Connection.SendRequest<ListEntitlementsRequest, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.listEntitlements, request);
            }
            public Task<Entitlements> ListEntitlementsAsync(ListEntitlementsRequest request)
            {
                return Connection.SendRequestAsync<ListEntitlementsRequest, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.listEntitlements, request);
            }

            public NullStruct HasEntitlement(HasEntitlementRequest request)
            {
                return Connection.SendRequest<HasEntitlementRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.hasEntitlement, request);
            }
            public Task<NullStruct> HasEntitlementAsync(HasEntitlementRequest request)
            {
                return Connection.SendRequestAsync<HasEntitlementRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.hasEntitlement, request);
            }

            public UseCount GetUseCount(GetUseCountRequest request)
            {
                return Connection.SendRequest<GetUseCountRequest, UseCount, NullStruct>(this, (ushort)AuthenticationComponentCommand.getUseCount, request);
            }
            public Task<UseCount> GetUseCountAsync(GetUseCountRequest request)
            {
                return Connection.SendRequestAsync<GetUseCountRequest, UseCount, NullStruct>(this, (ushort)AuthenticationComponentCommand.getUseCount, request);
            }

            public DecrementUseCount DecrementUseCount(DecrementUseCountRequest request)
            {
                return Connection.SendRequest<DecrementUseCountRequest, DecrementUseCount, NullStruct>(this, (ushort)AuthenticationComponentCommand.decrementUseCount, request);
            }
            public Task<DecrementUseCount> DecrementUseCountAsync(DecrementUseCountRequest request)
            {
                return Connection.SendRequestAsync<DecrementUseCountRequest, DecrementUseCount, NullStruct>(this, (ushort)AuthenticationComponentCommand.decrementUseCount, request);
            }

            public GetAuthTokenResponse GetAuthToken()
            {
                return Connection.SendRequest<NullStruct, GetAuthTokenResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAuthToken, new NullStruct());
            }
            public Task<GetAuthTokenResponse> GetAuthTokenAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GetAuthTokenResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAuthToken, new NullStruct());
            }

            public GetHandoffTokenResponse GetHandoffToken(GetHandoffTokenRequest request)
            {
                return Connection.SendRequest<GetHandoffTokenRequest, GetHandoffTokenResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getHandoffToken, request);
            }
            public Task<GetHandoffTokenResponse> GetHandoffTokenAsync(GetHandoffTokenRequest request)
            {
                return Connection.SendRequestAsync<GetHandoffTokenRequest, GetHandoffTokenResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getHandoffToken, request);
            }

            public PasswordRulesInfo GetPasswordRules()
            {
                return Connection.SendRequest<NullStruct, PasswordRulesInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPasswordRules, new NullStruct());
            }
            public Task<PasswordRulesInfo> GetPasswordRulesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, PasswordRulesInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPasswordRules, new NullStruct());
            }

            public GrantEntitlement2Response GrantEntitlement2(GrantEntitlement2Request request)
            {
                return Connection.SendRequest<GrantEntitlement2Request, GrantEntitlement2Response, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement2, request);
            }
            public Task<GrantEntitlement2Response> GrantEntitlement2Async(GrantEntitlement2Request request)
            {
                return Connection.SendRequestAsync<GrantEntitlement2Request, GrantEntitlement2Response, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement2, request);
            }

            public LoginResponse Login(LoginRequest request)
            {
                return Connection.SendRequest<LoginRequest, LoginResponse, CreateAccountResponse>(this, (ushort)AuthenticationComponentCommand.login, request);
            }
            public Task<LoginResponse> LoginAsync(LoginRequest request)
            {
                return Connection.SendRequestAsync<LoginRequest, LoginResponse, CreateAccountResponse>(this, (ushort)AuthenticationComponentCommand.login, request);
            }

            public NullStruct AcceptTos(AcceptTosRequest request)
            {
                return Connection.SendRequest<AcceptTosRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.acceptTos, request);
            }
            public Task<NullStruct> AcceptTosAsync(AcceptTosRequest request)
            {
                return Connection.SendRequestAsync<AcceptTosRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.acceptTos, request);
            }

            public GetTosInfoResponse GetTosInfo(GetTosInfoRequest request)
            {
                return Connection.SendRequest<GetTosInfoRequest, GetTosInfoResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTosInfo, request);
            }
            public Task<GetTosInfoResponse> GetTosInfoAsync(GetTosInfoRequest request)
            {
                return Connection.SendRequestAsync<GetTosInfoRequest, GetTosInfoResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTosInfo, request);
            }

            public NullStruct ModifyEntitlement2(ModifyEntitlement2Request request)
            {
                return Connection.SendRequest<ModifyEntitlement2Request, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.modifyEntitlement2, request);
            }
            public Task<NullStruct> ModifyEntitlement2Async(ModifyEntitlement2Request request)
            {
                return Connection.SendRequestAsync<ModifyEntitlement2Request, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.modifyEntitlement2, request);
            }

            public ConsumecodeResponse Consumecode(ConsumecodeRequest request)
            {
                return Connection.SendRequest<ConsumecodeRequest, ConsumecodeResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.consumecode, request);
            }
            public Task<ConsumecodeResponse> ConsumecodeAsync(ConsumecodeRequest request)
            {
                return Connection.SendRequestAsync<ConsumecodeRequest, ConsumecodeResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.consumecode, request);
            }

            public NullStruct PasswordForgot(PasswordForgotRequest request)
            {
                return Connection.SendRequest<PasswordForgotRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.passwordForgot, request);
            }
            public Task<NullStruct> PasswordForgotAsync(PasswordForgotRequest request)
            {
                return Connection.SendRequestAsync<PasswordForgotRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.passwordForgot, request);
            }

            public NullStruct GetTermsAndConditionsContent()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTermsAndConditionsContent, new NullStruct());
            }
            public Task<NullStruct> GetTermsAndConditionsContentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTermsAndConditionsContent, new NullStruct());
            }

            public GetLegalDocContentResponse GetPrivacyPolicyContent(GetLegalDocContentRequest request)
            {
                return Connection.SendRequest<GetLegalDocContentRequest, GetLegalDocContentResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPrivacyPolicyContent, request);
            }
            public Task<GetLegalDocContentResponse> GetPrivacyPolicyContentAsync(GetLegalDocContentRequest request)
            {
                return Connection.SendRequestAsync<GetLegalDocContentRequest, GetLegalDocContentResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPrivacyPolicyContent, request);
            }

            public NullStruct ListPersonaEntitlements2(ListPersonaEntitlements2Request request)
            {
                return Connection.SendRequest<ListPersonaEntitlements2Request, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.listPersonaEntitlements2, request);
            }
            public Task<NullStruct> ListPersonaEntitlements2Async(ListPersonaEntitlements2Request request)
            {
                return Connection.SendRequestAsync<ListPersonaEntitlements2Request, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.listPersonaEntitlements2, request);
            }

            public FullLoginResponse SilentLogin(SilentLoginRequest request)
            {
                return Connection.SendRequest<SilentLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.silentLogin, request);
            }
            public Task<FullLoginResponse> SilentLoginAsync(SilentLoginRequest request)
            {
                return Connection.SendRequestAsync<SilentLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.silentLogin, request);
            }

            public CheckAgeReqResponse CheckAgeReq(CheckAgeReqRequest request)
            {
                return Connection.SendRequest<CheckAgeReqRequest, CheckAgeReqResponse, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.checkAgeReq, request);
            }
            public Task<CheckAgeReqResponse> CheckAgeReqAsync(CheckAgeReqRequest request)
            {
                return Connection.SendRequestAsync<CheckAgeReqRequest, CheckAgeReqResponse, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.checkAgeReq, request);
            }

            public OptInValue GetOptIn()
            {
                return Connection.SendRequest<NullStruct, OptInValue, NullStruct>(this, (ushort)AuthenticationComponentCommand.getOptIn, new NullStruct());
            }
            public Task<OptInValue> GetOptInAsync()
            {
                return Connection.SendRequestAsync<NullStruct, OptInValue, NullStruct>(this, (ushort)AuthenticationComponentCommand.getOptIn, new NullStruct());
            }

            public NullStruct EnableOptIn()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.enableOptIn, new NullStruct());
            }
            public Task<NullStruct> EnableOptInAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.enableOptIn, new NullStruct());
            }

            public NullStruct DisableOptIn()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.disableOptIn, new NullStruct());
            }
            public Task<NullStruct> DisableOptInAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.disableOptIn, new NullStruct());
            }

            public FullLoginResponse ExpressLogin(ExpressLoginRequest request)
            {
                return Connection.SendRequest<ExpressLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.expressLogin, request);
            }
            public Task<FullLoginResponse> ExpressLoginAsync(ExpressLoginRequest request)
            {
                return Connection.SendRequestAsync<ExpressLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.expressLogin, request);
            }

            public NullStruct Logout()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.logout, new NullStruct());
            }
            public Task<NullStruct> LogoutAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.logout, new NullStruct());
            }

            public PersonaDetails CreatePersona(CreatePersonaRequest request)
            {
                return Connection.SendRequest<CreatePersonaRequest, PersonaDetails, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.createPersona, request);
            }
            public Task<PersonaDetails> CreatePersonaAsync(CreatePersonaRequest request)
            {
                return Connection.SendRequestAsync<CreatePersonaRequest, PersonaDetails, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.createPersona, request);
            }

            public GetPersonaResponse GetPersona()
            {
                return Connection.SendRequest<NullStruct, GetPersonaResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPersona, new NullStruct());
            }
            public Task<GetPersonaResponse> GetPersonaAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GetPersonaResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPersona, new NullStruct());
            }

            public ListPersonasResponse ListPersonas()
            {
                return Connection.SendRequest<NullStruct, ListPersonasResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.listPersonas, new NullStruct());
            }
            public Task<ListPersonasResponse> ListPersonasAsync()
            {
                return Connection.SendRequestAsync<NullStruct, ListPersonasResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.listPersonas, new NullStruct());
            }

            public SessionInfo LoginPersona(LoginPersonaRequest request)
            {
                return Connection.SendRequest<LoginPersonaRequest, SessionInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.loginPersona, request);
            }
            public Task<SessionInfo> LoginPersonaAsync(LoginPersonaRequest request)
            {
                return Connection.SendRequestAsync<LoginPersonaRequest, SessionInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.loginPersona, request);
            }

            public NullStruct LogoutPersona()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.logoutPersona, new NullStruct());
            }
            public Task<NullStruct> LogoutPersonaAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.logoutPersona, new NullStruct());
            }

            public NullStruct DeletePersona()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.deletePersona, new NullStruct());
            }
            public Task<NullStruct> DeletePersonaAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.deletePersona, new NullStruct());
            }

            public NullStruct DisablePersona(PersonaRequest request)
            {
                return Connection.SendRequest<PersonaRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.disablePersona, request);
            }
            public Task<NullStruct> DisablePersonaAsync(PersonaRequest request)
            {
                return Connection.SendRequestAsync<PersonaRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.disablePersona, request);
            }

            public ListDeviceAccountsResponse ListDeviceAccounts(ListDeviceAccountsRequest request)
            {
                return Connection.SendRequest<ListDeviceAccountsRequest, ListDeviceAccountsResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.listDeviceAccounts, request);
            }
            public Task<ListDeviceAccountsResponse> ListDeviceAccountsAsync(ListDeviceAccountsRequest request)
            {
                return Connection.SendRequestAsync<ListDeviceAccountsRequest, ListDeviceAccountsResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.listDeviceAccounts, request);
            }

            public ConsoleCreateAccountResponse XboxCreateAccount(ConsoleCreateAccountRequest request)
            {
                return Connection.SendRequest<ConsoleCreateAccountRequest, ConsoleCreateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxCreateAccount, request);
            }
            public Task<ConsoleCreateAccountResponse> XboxCreateAccountAsync(ConsoleCreateAccountRequest request)
            {
                return Connection.SendRequestAsync<ConsoleCreateAccountRequest, ConsoleCreateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxCreateAccount, request);
            }

            public FullLoginResponse OriginLogin(OriginLoginRequest request)
            {
                return Connection.SendRequest<OriginLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.originLogin, request);
            }
            public Task<FullLoginResponse> OriginLoginAsync(OriginLoginRequest request)
            {
                return Connection.SendRequestAsync<OriginLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.originLogin, request);
            }

            public NullStruct XboxAssociateAccount(ConsoleAssociateAccountRequest request)
            {
                return Connection.SendRequest<ConsoleAssociateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxAssociateAccount, request);
            }
            public Task<NullStruct> XboxAssociateAccountAsync(ConsoleAssociateAccountRequest request)
            {
                return Connection.SendRequestAsync<ConsoleAssociateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxAssociateAccount, request);
            }

            public ConsoleLoginResponse XboxLogin(XboxLoginRequest request)
            {
                return Connection.SendRequest<XboxLoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxLogin, request);
            }
            public Task<ConsoleLoginResponse> XboxLoginAsync(XboxLoginRequest request)
            {
                return Connection.SendRequestAsync<XboxLoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxLogin, request);
            }

            public ConsoleCreateAccountResponse Ps3CreateAccount(ConsoleCreateAccountRequest request)
            {
                return Connection.SendRequest<ConsoleCreateAccountRequest, ConsoleCreateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3CreateAccount, request);
            }
            public Task<ConsoleCreateAccountResponse> Ps3CreateAccountAsync(ConsoleCreateAccountRequest request)
            {
                return Connection.SendRequestAsync<ConsoleCreateAccountRequest, ConsoleCreateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3CreateAccount, request);
            }

            public NullStruct Ps3AssociateAccount(ConsoleAssociateAccountRequest request)
            {
                return Connection.SendRequest<ConsoleAssociateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3AssociateAccount, request);
            }
            public Task<NullStruct> Ps3AssociateAccountAsync(ConsoleAssociateAccountRequest request)
            {
                return Connection.SendRequestAsync<ConsoleAssociateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3AssociateAccount, request);
            }

            public ConsoleLoginResponse Ps3Login(PS3LoginRequest request)
            {
                return Connection.SendRequest<PS3LoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3Login, request);
            }
            public Task<ConsoleLoginResponse> Ps3LoginAsync(PS3LoginRequest request)
            {
                return Connection.SendRequestAsync<PS3LoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3Login, request);
            }

            public NullStruct ValidateSessionKey(ValidateSessionKeyRequest request)
            {
                return Connection.SendRequest<ValidateSessionKeyRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.validateSessionKey, request);
            }
            public Task<NullStruct> ValidateSessionKeyAsync(ValidateSessionKeyRequest request)
            {
                return Connection.SendRequestAsync<ValidateSessionKeyRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.validateSessionKey, request);
            }

            public NullStruct CreateWalUserSession()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.createWalUserSession, new NullStruct());
            }
            public Task<NullStruct> CreateWalUserSessionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.createWalUserSession, new NullStruct());
            }

            public NullStruct AcceptLegalDocs(AcceptLegalDocsRequest request)
            {
                return Connection.SendRequest<AcceptLegalDocsRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.acceptLegalDocs, request);
            }
            public Task<NullStruct> AcceptLegalDocsAsync(AcceptLegalDocsRequest request)
            {
                return Connection.SendRequestAsync<AcceptLegalDocsRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.acceptLegalDocs, request);
            }

            public GetLegalDocsInfoResponse GetLegalDocsInfo(GetLegalDocsInfoRequest request)
            {
                return Connection.SendRequest<GetLegalDocsInfoRequest, GetLegalDocsInfoResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getLegalDocsInfo, request);
            }
            public Task<GetLegalDocsInfoResponse> GetLegalDocsInfoAsync(GetLegalDocsInfoRequest request)
            {
                return Connection.SendRequestAsync<GetLegalDocsInfoRequest, GetLegalDocsInfoResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getLegalDocsInfo, request);
            }

            public GetLegalDocContentResponse GetTermsOfServiceContent(GetLegalDocContentRequest request)
            {
                return Connection.SendRequest<GetLegalDocContentRequest, GetLegalDocContentResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTermsOfServiceContent, request);
            }
            public Task<GetLegalDocContentResponse> GetTermsOfServiceContentAsync(GetLegalDocContentRequest request)
            {
                return Connection.SendRequestAsync<GetLegalDocContentRequest, GetLegalDocContentResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTermsOfServiceContent, request);
            }

            public ConsoleLoginResponse DeviceLoginGuest(DeviceLoginGuestRequest request)
            {
                return Connection.SendRequest<DeviceLoginGuestRequest, ConsoleLoginResponse, ConsoleCreateAccountRequest>(this, (ushort)AuthenticationComponentCommand.deviceLoginGuest, request);
            }
            public Task<ConsoleLoginResponse> DeviceLoginGuestAsync(DeviceLoginGuestRequest request)
            {
                return Connection.SendRequestAsync<DeviceLoginGuestRequest, ConsoleLoginResponse, ConsoleCreateAccountRequest>(this, (ushort)AuthenticationComponentCommand.deviceLoginGuest, request);
            }

            public NullStruct CheckSinglePlayerLogin()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.checkSinglePlayerLogin, new NullStruct());
            }
            public Task<NullStruct> CheckSinglePlayerLoginAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.checkSinglePlayerLogin, new NullStruct());
            }


            public override Type GetCommandRequestType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AuthenticationComponentNotification notification) => AuthenticationComponentBase.GetNotificationType(notification);

        }

        public class Proxy : BlazeProxyComponent<AuthenticationComponentCommand, AuthenticationComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(AuthenticationComponentBase.Id, AuthenticationComponentBase.Name)
            {

            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.createAccount)]
            public virtual Task<CreateAccountResponse> CreateAccountAsync(CreateAccountParameters request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<CreateAccountParameters, CreateAccountResponse, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.createAccount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.updateAccount)]
            public virtual Task<UpdateAccountResponse> UpdateAccountAsync(UpdateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateAccountRequest, UpdateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateAccount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.updateParentalEmail)]
            public virtual Task<NullStruct> UpdateParentalEmailAsync(UpdateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateParentalEmail, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listUserEntitlements2)]
            public virtual Task<Entitlements> ListUserEntitlements2Async(ListUserEntitlements2Request request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ListUserEntitlements2Request, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.listUserEntitlements2, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getAccount)]
            public virtual Task<AccountInfo> GetAccountAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, AccountInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAccount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.grantEntitlement)]
            public virtual Task<NullStruct> GrantEntitlementAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listEntitlements)]
            public virtual Task<Entitlements> ListEntitlementsAsync(ListEntitlementsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ListEntitlementsRequest, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.listEntitlements, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.hasEntitlement)]
            public virtual Task<NullStruct> HasEntitlementAsync(HasEntitlementRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<HasEntitlementRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.hasEntitlement, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getUseCount)]
            public virtual Task<UseCount> GetUseCountAsync(GetUseCountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetUseCountRequest, UseCount, NullStruct>(this, (ushort)AuthenticationComponentCommand.getUseCount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.decrementUseCount)]
            public virtual Task<DecrementUseCount> DecrementUseCountAsync(DecrementUseCountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<DecrementUseCountRequest, DecrementUseCount, NullStruct>(this, (ushort)AuthenticationComponentCommand.decrementUseCount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getAuthToken)]
            public virtual Task<GetAuthTokenResponse> GetAuthTokenAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GetAuthTokenResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAuthToken, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getHandoffToken)]
            public virtual Task<GetHandoffTokenResponse> GetHandoffTokenAsync(GetHandoffTokenRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetHandoffTokenRequest, GetHandoffTokenResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getHandoffToken, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getPasswordRules)]
            public virtual Task<PasswordRulesInfo> GetPasswordRulesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, PasswordRulesInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPasswordRules, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.grantEntitlement2)]
            public virtual Task<GrantEntitlement2Response> GrantEntitlement2Async(GrantEntitlement2Request request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GrantEntitlement2Request, GrantEntitlement2Response, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement2, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.login)]
            public virtual Task<LoginResponse> LoginAsync(LoginRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<LoginRequest, LoginResponse, CreateAccountResponse>(this, (ushort)AuthenticationComponentCommand.login, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.acceptTos)]
            public virtual Task<NullStruct> AcceptTosAsync(AcceptTosRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<AcceptTosRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.acceptTos, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getTosInfo)]
            public virtual Task<GetTosInfoResponse> GetTosInfoAsync(GetTosInfoRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetTosInfoRequest, GetTosInfoResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTosInfo, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.modifyEntitlement2)]
            public virtual Task<NullStruct> ModifyEntitlement2Async(ModifyEntitlement2Request request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ModifyEntitlement2Request, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.modifyEntitlement2, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.consumecode)]
            public virtual Task<ConsumecodeResponse> ConsumecodeAsync(ConsumecodeRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ConsumecodeRequest, ConsumecodeResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.consumecode, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.passwordForgot)]
            public virtual Task<NullStruct> PasswordForgotAsync(PasswordForgotRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<PasswordForgotRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.passwordForgot, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getTermsAndConditionsContent)]
            public virtual Task<NullStruct> GetTermsAndConditionsContentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTermsAndConditionsContent, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getPrivacyPolicyContent)]
            public virtual Task<GetLegalDocContentResponse> GetPrivacyPolicyContentAsync(GetLegalDocContentRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetLegalDocContentRequest, GetLegalDocContentResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPrivacyPolicyContent, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listPersonaEntitlements2)]
            public virtual Task<NullStruct> ListPersonaEntitlements2Async(ListPersonaEntitlements2Request request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ListPersonaEntitlements2Request, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.listPersonaEntitlements2, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.silentLogin)]
            public virtual Task<FullLoginResponse> SilentLoginAsync(SilentLoginRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SilentLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.silentLogin, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.checkAgeReq)]
            public virtual Task<CheckAgeReqResponse> CheckAgeReqAsync(CheckAgeReqRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<CheckAgeReqRequest, CheckAgeReqResponse, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.checkAgeReq, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getOptIn)]
            public virtual Task<OptInValue> GetOptInAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, OptInValue, NullStruct>(this, (ushort)AuthenticationComponentCommand.getOptIn, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.enableOptIn)]
            public virtual Task<NullStruct> EnableOptInAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.enableOptIn, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.disableOptIn)]
            public virtual Task<NullStruct> DisableOptInAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.disableOptIn, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.expressLogin)]
            public virtual Task<FullLoginResponse> ExpressLoginAsync(ExpressLoginRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ExpressLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.expressLogin, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.logout)]
            public virtual Task<NullStruct> LogoutAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.logout, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.createPersona)]
            public virtual Task<PersonaDetails> CreatePersonaAsync(CreatePersonaRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<CreatePersonaRequest, PersonaDetails, FieldValidateErrorList>(this, (ushort)AuthenticationComponentCommand.createPersona, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getPersona)]
            public virtual Task<GetPersonaResponse> GetPersonaAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GetPersonaResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPersona, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listPersonas)]
            public virtual Task<ListPersonasResponse> ListPersonasAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, ListPersonasResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.listPersonas, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.loginPersona)]
            public virtual Task<SessionInfo> LoginPersonaAsync(LoginPersonaRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<LoginPersonaRequest, SessionInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.loginPersona, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.logoutPersona)]
            public virtual Task<NullStruct> LogoutPersonaAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.logoutPersona, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.deletePersona)]
            public virtual Task<NullStruct> DeletePersonaAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.deletePersona, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.disablePersona)]
            public virtual Task<NullStruct> DisablePersonaAsync(PersonaRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<PersonaRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.disablePersona, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.listDeviceAccounts)]
            public virtual Task<ListDeviceAccountsResponse> ListDeviceAccountsAsync(ListDeviceAccountsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ListDeviceAccountsRequest, ListDeviceAccountsResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.listDeviceAccounts, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxCreateAccount)]
            public virtual Task<ConsoleCreateAccountResponse> XboxCreateAccountAsync(ConsoleCreateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ConsoleCreateAccountRequest, ConsoleCreateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxCreateAccount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.originLogin)]
            public virtual Task<FullLoginResponse> OriginLoginAsync(OriginLoginRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<OriginLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.originLogin, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxAssociateAccount)]
            public virtual Task<NullStruct> XboxAssociateAccountAsync(ConsoleAssociateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ConsoleAssociateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxAssociateAccount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxLogin)]
            public virtual Task<ConsoleLoginResponse> XboxLoginAsync(XboxLoginRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<XboxLoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxLogin, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3CreateAccount)]
            public virtual Task<ConsoleCreateAccountResponse> Ps3CreateAccountAsync(ConsoleCreateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ConsoleCreateAccountRequest, ConsoleCreateAccountResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3CreateAccount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3AssociateAccount)]
            public virtual Task<NullStruct> Ps3AssociateAccountAsync(ConsoleAssociateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ConsoleAssociateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3AssociateAccount, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3Login)]
            public virtual Task<ConsoleLoginResponse> Ps3LoginAsync(PS3LoginRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<PS3LoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3Login, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.validateSessionKey)]
            public virtual Task<NullStruct> ValidateSessionKeyAsync(ValidateSessionKeyRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ValidateSessionKeyRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.validateSessionKey, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.createWalUserSession)]
            public virtual Task<NullStruct> CreateWalUserSessionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.createWalUserSession, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.acceptLegalDocs)]
            public virtual Task<NullStruct> AcceptLegalDocsAsync(AcceptLegalDocsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<AcceptLegalDocsRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.acceptLegalDocs, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getLegalDocsInfo)]
            public virtual Task<GetLegalDocsInfoResponse> GetLegalDocsInfoAsync(GetLegalDocsInfoRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetLegalDocsInfoRequest, GetLegalDocsInfoResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getLegalDocsInfo, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.getTermsOfServiceContent)]
            public virtual Task<GetLegalDocContentResponse> GetTermsOfServiceContentAsync(GetLegalDocContentRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetLegalDocContentRequest, GetLegalDocContentResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.getTermsOfServiceContent, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.deviceLoginGuest)]
            public virtual Task<ConsoleLoginResponse> DeviceLoginGuestAsync(DeviceLoginGuestRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<DeviceLoginGuestRequest, ConsoleLoginResponse, ConsoleCreateAccountRequest>(this, (ushort)AuthenticationComponentCommand.deviceLoginGuest, request);
            }

            [BlazeCommand((ushort)AuthenticationComponentCommand.checkSinglePlayerLogin)]
            public virtual Task<NullStruct> CheckSinglePlayerLoginAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.checkSinglePlayerLogin, request);
            }


            public override Type GetCommandRequestType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AuthenticationComponentNotification notification) => AuthenticationComponentBase.GetNotificationType(notification);

        }

        public static Type GetCommandRequestType(AuthenticationComponentCommand command) => command switch
        {
            AuthenticationComponentCommand.createAccount => typeof(CreateAccountParameters),
            AuthenticationComponentCommand.updateAccount => typeof(UpdateAccountRequest),
            AuthenticationComponentCommand.updateParentalEmail => typeof(UpdateAccountRequest),
            AuthenticationComponentCommand.listUserEntitlements2 => typeof(ListUserEntitlements2Request),
            AuthenticationComponentCommand.getAccount => typeof(NullStruct),
            AuthenticationComponentCommand.grantEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.listEntitlements => typeof(ListEntitlementsRequest),
            AuthenticationComponentCommand.hasEntitlement => typeof(HasEntitlementRequest),
            AuthenticationComponentCommand.getUseCount => typeof(GetUseCountRequest),
            AuthenticationComponentCommand.decrementUseCount => typeof(DecrementUseCountRequest),
            AuthenticationComponentCommand.getAuthToken => typeof(NullStruct),
            AuthenticationComponentCommand.getHandoffToken => typeof(GetHandoffTokenRequest),
            AuthenticationComponentCommand.getPasswordRules => typeof(NullStruct),
            AuthenticationComponentCommand.grantEntitlement2 => typeof(GrantEntitlement2Request),
            AuthenticationComponentCommand.login => typeof(LoginRequest),
            AuthenticationComponentCommand.acceptTos => typeof(AcceptTosRequest),
            AuthenticationComponentCommand.getTosInfo => typeof(GetTosInfoRequest),
            AuthenticationComponentCommand.modifyEntitlement2 => typeof(ModifyEntitlement2Request),
            AuthenticationComponentCommand.consumecode => typeof(ConsumecodeRequest),
            AuthenticationComponentCommand.passwordForgot => typeof(PasswordForgotRequest),
            AuthenticationComponentCommand.getTermsAndConditionsContent => typeof(NullStruct),
            AuthenticationComponentCommand.getPrivacyPolicyContent => typeof(GetLegalDocContentRequest),
            AuthenticationComponentCommand.listPersonaEntitlements2 => typeof(ListPersonaEntitlements2Request),
            AuthenticationComponentCommand.silentLogin => typeof(SilentLoginRequest),
            AuthenticationComponentCommand.checkAgeReq => typeof(CheckAgeReqRequest),
            AuthenticationComponentCommand.getOptIn => typeof(NullStruct),
            AuthenticationComponentCommand.enableOptIn => typeof(NullStruct),
            AuthenticationComponentCommand.disableOptIn => typeof(NullStruct),
            AuthenticationComponentCommand.expressLogin => typeof(ExpressLoginRequest),
            AuthenticationComponentCommand.logout => typeof(NullStruct),
            AuthenticationComponentCommand.createPersona => typeof(CreatePersonaRequest),
            AuthenticationComponentCommand.getPersona => typeof(NullStruct),
            AuthenticationComponentCommand.listPersonas => typeof(NullStruct),
            AuthenticationComponentCommand.loginPersona => typeof(LoginPersonaRequest),
            AuthenticationComponentCommand.logoutPersona => typeof(NullStruct),
            AuthenticationComponentCommand.deletePersona => typeof(NullStruct),
            AuthenticationComponentCommand.disablePersona => typeof(PersonaRequest),
            AuthenticationComponentCommand.listDeviceAccounts => typeof(ListDeviceAccountsRequest),
            AuthenticationComponentCommand.xboxCreateAccount => typeof(ConsoleCreateAccountRequest),
            AuthenticationComponentCommand.originLogin => typeof(OriginLoginRequest),
            AuthenticationComponentCommand.xboxAssociateAccount => typeof(ConsoleAssociateAccountRequest),
            AuthenticationComponentCommand.xboxLogin => typeof(XboxLoginRequest),
            AuthenticationComponentCommand.ps3CreateAccount => typeof(ConsoleCreateAccountRequest),
            AuthenticationComponentCommand.ps3AssociateAccount => typeof(ConsoleAssociateAccountRequest),
            AuthenticationComponentCommand.ps3Login => typeof(PS3LoginRequest),
            AuthenticationComponentCommand.validateSessionKey => typeof(ValidateSessionKeyRequest),
            AuthenticationComponentCommand.createWalUserSession => typeof(NullStruct),
            AuthenticationComponentCommand.acceptLegalDocs => typeof(AcceptLegalDocsRequest),
            AuthenticationComponentCommand.getLegalDocsInfo => typeof(GetLegalDocsInfoRequest),
            AuthenticationComponentCommand.getTermsOfServiceContent => typeof(GetLegalDocContentRequest),
            AuthenticationComponentCommand.deviceLoginGuest => typeof(DeviceLoginGuestRequest),
            AuthenticationComponentCommand.checkSinglePlayerLogin => typeof(NullStruct),
            _ => typeof(NullStruct)
        };

        public static Type GetCommandResponseType(AuthenticationComponentCommand command) => command switch
        {
            AuthenticationComponentCommand.createAccount => typeof(CreateAccountResponse),
            AuthenticationComponentCommand.updateAccount => typeof(UpdateAccountResponse),
            AuthenticationComponentCommand.updateParentalEmail => typeof(NullStruct),
            AuthenticationComponentCommand.listUserEntitlements2 => typeof(Entitlements),
            AuthenticationComponentCommand.getAccount => typeof(AccountInfo),
            AuthenticationComponentCommand.grantEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.listEntitlements => typeof(Entitlements),
            AuthenticationComponentCommand.hasEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.getUseCount => typeof(UseCount),
            AuthenticationComponentCommand.decrementUseCount => typeof(DecrementUseCount),
            AuthenticationComponentCommand.getAuthToken => typeof(GetAuthTokenResponse),
            AuthenticationComponentCommand.getHandoffToken => typeof(GetHandoffTokenResponse),
            AuthenticationComponentCommand.getPasswordRules => typeof(PasswordRulesInfo),
            AuthenticationComponentCommand.grantEntitlement2 => typeof(GrantEntitlement2Response),
            AuthenticationComponentCommand.login => typeof(LoginResponse),
            AuthenticationComponentCommand.acceptTos => typeof(NullStruct),
            AuthenticationComponentCommand.getTosInfo => typeof(GetTosInfoResponse),
            AuthenticationComponentCommand.modifyEntitlement2 => typeof(NullStruct),
            AuthenticationComponentCommand.consumecode => typeof(ConsumecodeResponse),
            AuthenticationComponentCommand.passwordForgot => typeof(NullStruct),
            AuthenticationComponentCommand.getTermsAndConditionsContent => typeof(NullStruct),
            AuthenticationComponentCommand.getPrivacyPolicyContent => typeof(GetLegalDocContentResponse),
            AuthenticationComponentCommand.listPersonaEntitlements2 => typeof(NullStruct),
            AuthenticationComponentCommand.silentLogin => typeof(FullLoginResponse),
            AuthenticationComponentCommand.checkAgeReq => typeof(CheckAgeReqResponse),
            AuthenticationComponentCommand.getOptIn => typeof(OptInValue),
            AuthenticationComponentCommand.enableOptIn => typeof(NullStruct),
            AuthenticationComponentCommand.disableOptIn => typeof(NullStruct),
            AuthenticationComponentCommand.expressLogin => typeof(FullLoginResponse),
            AuthenticationComponentCommand.logout => typeof(NullStruct),
            AuthenticationComponentCommand.createPersona => typeof(PersonaDetails),
            AuthenticationComponentCommand.getPersona => typeof(GetPersonaResponse),
            AuthenticationComponentCommand.listPersonas => typeof(ListPersonasResponse),
            AuthenticationComponentCommand.loginPersona => typeof(SessionInfo),
            AuthenticationComponentCommand.logoutPersona => typeof(NullStruct),
            AuthenticationComponentCommand.deletePersona => typeof(NullStruct),
            AuthenticationComponentCommand.disablePersona => typeof(NullStruct),
            AuthenticationComponentCommand.listDeviceAccounts => typeof(ListDeviceAccountsResponse),
            AuthenticationComponentCommand.xboxCreateAccount => typeof(ConsoleCreateAccountResponse),
            AuthenticationComponentCommand.originLogin => typeof(FullLoginResponse),
            AuthenticationComponentCommand.xboxAssociateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.xboxLogin => typeof(ConsoleLoginResponse),
            AuthenticationComponentCommand.ps3CreateAccount => typeof(ConsoleCreateAccountResponse),
            AuthenticationComponentCommand.ps3AssociateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.ps3Login => typeof(ConsoleLoginResponse),
            AuthenticationComponentCommand.validateSessionKey => typeof(NullStruct),
            AuthenticationComponentCommand.createWalUserSession => typeof(NullStruct),
            AuthenticationComponentCommand.acceptLegalDocs => typeof(NullStruct),
            AuthenticationComponentCommand.getLegalDocsInfo => typeof(GetLegalDocsInfoResponse),
            AuthenticationComponentCommand.getTermsOfServiceContent => typeof(GetLegalDocContentResponse),
            AuthenticationComponentCommand.deviceLoginGuest => typeof(ConsoleLoginResponse),
            AuthenticationComponentCommand.checkSinglePlayerLogin => typeof(NullStruct),
            _ => typeof(NullStruct)
        };

        public static Type GetCommandErrorResponseType(AuthenticationComponentCommand command) => command switch
        {
            AuthenticationComponentCommand.createAccount => typeof(FieldValidateErrorList),
            AuthenticationComponentCommand.updateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.updateParentalEmail => typeof(NullStruct),
            AuthenticationComponentCommand.listUserEntitlements2 => typeof(NullStruct),
            AuthenticationComponentCommand.getAccount => typeof(NullStruct),
            AuthenticationComponentCommand.grantEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.listEntitlements => typeof(NullStruct),
            AuthenticationComponentCommand.hasEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.getUseCount => typeof(NullStruct),
            AuthenticationComponentCommand.decrementUseCount => typeof(NullStruct),
            AuthenticationComponentCommand.getAuthToken => typeof(NullStruct),
            AuthenticationComponentCommand.getHandoffToken => typeof(NullStruct),
            AuthenticationComponentCommand.getPasswordRules => typeof(NullStruct),
            AuthenticationComponentCommand.grantEntitlement2 => typeof(NullStruct),
            AuthenticationComponentCommand.login => typeof(CreateAccountResponse),
            AuthenticationComponentCommand.acceptTos => typeof(NullStruct),
            AuthenticationComponentCommand.getTosInfo => typeof(NullStruct),
            AuthenticationComponentCommand.modifyEntitlement2 => typeof(NullStruct),
            AuthenticationComponentCommand.consumecode => typeof(NullStruct),
            AuthenticationComponentCommand.passwordForgot => typeof(NullStruct),
            AuthenticationComponentCommand.getTermsAndConditionsContent => typeof(NullStruct),
            AuthenticationComponentCommand.getPrivacyPolicyContent => typeof(NullStruct),
            AuthenticationComponentCommand.listPersonaEntitlements2 => typeof(NullStruct),
            AuthenticationComponentCommand.silentLogin => typeof(NullStruct),
            AuthenticationComponentCommand.checkAgeReq => typeof(FieldValidateErrorList),
            AuthenticationComponentCommand.getOptIn => typeof(NullStruct),
            AuthenticationComponentCommand.enableOptIn => typeof(NullStruct),
            AuthenticationComponentCommand.disableOptIn => typeof(NullStruct),
            AuthenticationComponentCommand.expressLogin => typeof(NullStruct),
            AuthenticationComponentCommand.logout => typeof(NullStruct),
            AuthenticationComponentCommand.createPersona => typeof(FieldValidateErrorList),
            AuthenticationComponentCommand.getPersona => typeof(NullStruct),
            AuthenticationComponentCommand.listPersonas => typeof(NullStruct),
            AuthenticationComponentCommand.loginPersona => typeof(NullStruct),
            AuthenticationComponentCommand.logoutPersona => typeof(NullStruct),
            AuthenticationComponentCommand.deletePersona => typeof(NullStruct),
            AuthenticationComponentCommand.disablePersona => typeof(NullStruct),
            AuthenticationComponentCommand.listDeviceAccounts => typeof(NullStruct),
            AuthenticationComponentCommand.xboxCreateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.originLogin => typeof(NullStruct),
            AuthenticationComponentCommand.xboxAssociateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.xboxLogin => typeof(NullStruct),
            AuthenticationComponentCommand.ps3CreateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.ps3AssociateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.ps3Login => typeof(NullStruct),
            AuthenticationComponentCommand.validateSessionKey => typeof(NullStruct),
            AuthenticationComponentCommand.createWalUserSession => typeof(NullStruct),
            AuthenticationComponentCommand.acceptLegalDocs => typeof(NullStruct),
            AuthenticationComponentCommand.getLegalDocsInfo => typeof(NullStruct),
            AuthenticationComponentCommand.getTermsOfServiceContent => typeof(NullStruct),
            AuthenticationComponentCommand.deviceLoginGuest => typeof(ConsoleCreateAccountRequest),
            AuthenticationComponentCommand.checkSinglePlayerLogin => typeof(NullStruct),
            _ => typeof(NullStruct)
        };

        public static Type GetNotificationType(AuthenticationComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };

        public enum AuthenticationComponentCommand : ushort
        {
            createAccount = 10,
            updateAccount = 20,
            updateParentalEmail = 28,
            listUserEntitlements2 = 29,
            getAccount = 30,
            grantEntitlement = 31,
            listEntitlements = 32,
            hasEntitlement = 33,
            getUseCount = 34,
            decrementUseCount = 35,
            getAuthToken = 36,
            getHandoffToken = 37,
            getPasswordRules = 38,
            grantEntitlement2 = 39,
            login = 40,
            acceptTos = 41,
            getTosInfo = 42,
            modifyEntitlement2 = 43,
            consumecode = 44,
            passwordForgot = 45,
            getTermsAndConditionsContent = 46,
            getPrivacyPolicyContent = 47,
            listPersonaEntitlements2 = 48,
            silentLogin = 50,
            checkAgeReq = 51,
            getOptIn = 52,
            enableOptIn = 53,
            disableOptIn = 54,
            expressLogin = 60,
            logout = 70,
            createPersona = 80,
            getPersona = 90,
            listPersonas = 100,
            loginPersona = 110,
            logoutPersona = 120,
            deletePersona = 140,
            disablePersona = 141,
            listDeviceAccounts = 143,
            xboxCreateAccount = 150,
            originLogin = 152,
            xboxAssociateAccount = 160,
            xboxLogin = 170,
            ps3CreateAccount = 180,
            ps3AssociateAccount = 190,
            ps3Login = 200,
            validateSessionKey = 210,
            createWalUserSession = 230,
            acceptLegalDocs = 241,
            getLegalDocsInfo = 242,
            getTermsOfServiceContent = 246,
            deviceLoginGuest = 300,
            checkSinglePlayerLogin = 500,
        }

        public enum AuthenticationComponentNotification : ushort
        {
        }

    }
}
