using Blaze2SDK.Blaze.Authentication;
using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class AuthenticationComponentBase
    {
        public const ushort Id = 1;
        public const string Name = "AuthenticationComponent";
        
        public class Server : BlazeServerComponent<AuthenticationComponentCommand, AuthenticationComponentNotification, Blaze2RpcError>
        {
            public Server() : base(AuthenticationComponentBase.Id, AuthenticationComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.createAccount)]
            public virtual Task<CreateAccountResponse> CreateAccountAsync(CreateAccountParameters request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.updateAccount)]
            public virtual Task<NullStruct> UpdateAccountAsync(UpdateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.updateParentalEmail)]
            public virtual Task<NullStruct> UpdateParentalEmailAsync(UpdateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getAccount)]
            public virtual Task<AccountInfo> GetAccountAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.grantEntitlement)]
            public virtual Task<NullStruct> GrantEntitlementAsync(Entitlement request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getEntitlements)]
            public virtual Task<Entitlements> GetEntitlementsAsync(ListEntitlementsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.hasEntitlement)]
            public virtual Task<NullStruct> HasEntitlementAsync(HasEntitlementRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getUseCount)]
            public virtual Task<NullStruct> GetUseCountAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.decrementUseCount)]
            public virtual Task<NullStruct> DecrementUseCountAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getAuthToken)]
            public virtual Task<NullStruct> GetAuthTokenAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getHandoffToken)]
            public virtual Task<NullStruct> GetHandoffTokenAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.listEntitlement2)]
            public virtual Task<NullStruct> ListEntitlement2Async(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.login)]
            public virtual Task<LoginResponse> LoginAsync(LoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.acceptTos)]
            public virtual Task<NullStruct> AcceptTosAsync(AcceptTosRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getTosInfo)]
            public virtual Task<GetTosInfoResponse> GetTosInfoAsync(GetTosInfoRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.consumecode)]
            public virtual Task<ConsumecodeResponse> ConsumecodeAsync(ConsumecodeRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.passwordForgot)]
            public virtual Task<NullStruct> PasswordForgotAsync(PasswordForgotRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.silentLogin)]
            public virtual Task<FullLoginResponse> SilentLoginAsync(SilentLoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.checkAgeReq)]
            public virtual Task<CheckAgeReqResponse> CheckAgeReqAsync(CheckAgeReqRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.expressLogin)]
            public virtual Task<FullLoginResponse> ExpressLoginAsync(ExpressLoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.stressLogin)]
            public virtual Task<NullStruct> StressLoginAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.logout)]
            public virtual Task<NullStruct> LogoutAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.createPersona)]
            public virtual Task<PersonaDetails> CreatePersonaAsync(CreatePersonaRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getPersona)]
            public virtual Task<NullStruct> GetPersonaAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.listPersonas)]
            public virtual Task<ListPersonasResponse> ListPersonasAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.loginPersona)]
            public virtual Task<SessionInfo> LoginPersonaAsync(LoginPersonaRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.logoutPersona)]
            public virtual Task<NullStruct> LogoutPersonaAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.deletePersona)]
            public virtual Task<NullStruct> DeletePersonaAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.disablePersona)]
            public virtual Task<PersonaDetails> DisablePersonaAsync(PersonaRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.listDeviceAccounts)]
            public virtual Task<ListDeviceAccountsResponse> ListDeviceAccountsAsync(ListDeviceAccountsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxCreateAccount)]
            public virtual Task<ConsoleCreateAccountResponse> XboxCreateAccountAsync(ConsoleCreateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxAssociateAccount)]
            public virtual Task<NullStruct> XboxAssociateAccountAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxLogin)]
            public virtual Task<ConsoleLoginResponse> XboxLoginAsync(XboxLoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3CreateAccount)]
            public virtual Task<ConsoleCreateAccountResponse> Ps3CreateAccountAsync(ConsoleCreateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3AssociateAccount)]
            public virtual Task<SessionInfo> Ps3AssociateAccountAsync(ConsoleAssociateAccountRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3Login)]
            public virtual Task<ConsoleLoginResponse> Ps3LoginAsync(PS3LoginRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.validateSessionKey)]
            public virtual Task<NullStruct> ValidateSessionKeyAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.createWalUserSession)]
            public virtual Task<NullStruct> CreateWalUserSessionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.deviceLoginGuest)]
            public virtual Task<ConsoleLoginResponse> DeviceLoginGuestAsync(DeviceLoginGuestRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AuthenticationComponentNotification notification) => AuthenticationComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<AuthenticationComponentCommand, AuthenticationComponentNotification, Blaze2RpcError>
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
            
            public NullStruct UpdateAccount(UpdateAccountRequest request)
            {
                return Connection.SendRequest<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateAccount, request);
            }
            public Task<NullStruct> UpdateAccountAsync(UpdateAccountRequest request)
            {
                return Connection.SendRequestAsync<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateAccount, request);
            }
            
            public NullStruct UpdateParentalEmail(UpdateAccountRequest request)
            {
                return Connection.SendRequest<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateParentalEmail, request);
            }
            public Task<NullStruct> UpdateParentalEmailAsync(UpdateAccountRequest request)
            {
                return Connection.SendRequestAsync<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateParentalEmail, request);
            }
            
            public AccountInfo GetAccount()
            {
                return Connection.SendRequest<NullStruct, AccountInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAccount, new NullStruct());
            }
            public Task<AccountInfo> GetAccountAsync()
            {
                return Connection.SendRequestAsync<NullStruct, AccountInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAccount, new NullStruct());
            }
            
            public NullStruct GrantEntitlement(Entitlement request)
            {
                return Connection.SendRequest<Entitlement, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement, request);
            }
            public Task<NullStruct> GrantEntitlementAsync(Entitlement request)
            {
                return Connection.SendRequestAsync<Entitlement, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement, request);
            }
            
            public Entitlements GetEntitlements(ListEntitlementsRequest request)
            {
                return Connection.SendRequest<ListEntitlementsRequest, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.getEntitlements, request);
            }
            public Task<Entitlements> GetEntitlementsAsync(ListEntitlementsRequest request)
            {
                return Connection.SendRequestAsync<ListEntitlementsRequest, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.getEntitlements, request);
            }
            
            public NullStruct HasEntitlement(HasEntitlementRequest request)
            {
                return Connection.SendRequest<HasEntitlementRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.hasEntitlement, request);
            }
            public Task<NullStruct> HasEntitlementAsync(HasEntitlementRequest request)
            {
                return Connection.SendRequestAsync<HasEntitlementRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.hasEntitlement, request);
            }
            
            public NullStruct GetUseCount()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getUseCount, new NullStruct());
            }
            public Task<NullStruct> GetUseCountAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getUseCount, new NullStruct());
            }
            
            public NullStruct DecrementUseCount()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.decrementUseCount, new NullStruct());
            }
            public Task<NullStruct> DecrementUseCountAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.decrementUseCount, new NullStruct());
            }
            
            public NullStruct GetAuthToken()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAuthToken, new NullStruct());
            }
            public Task<NullStruct> GetAuthTokenAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAuthToken, new NullStruct());
            }
            
            public NullStruct GetHandoffToken()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getHandoffToken, new NullStruct());
            }
            public Task<NullStruct> GetHandoffTokenAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getHandoffToken, new NullStruct());
            }
            
            public NullStruct ListEntitlement2()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.listEntitlement2, new NullStruct());
            }
            public Task<NullStruct> ListEntitlement2Async()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.listEntitlement2, new NullStruct());
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
            
            public FullLoginResponse ExpressLogin(ExpressLoginRequest request)
            {
                return Connection.SendRequest<ExpressLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.expressLogin, request);
            }
            public Task<FullLoginResponse> ExpressLoginAsync(ExpressLoginRequest request)
            {
                return Connection.SendRequestAsync<ExpressLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.expressLogin, request);
            }
            
            public NullStruct StressLogin()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.stressLogin, new NullStruct());
            }
            public Task<NullStruct> StressLoginAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.stressLogin, new NullStruct());
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
            
            public NullStruct GetPersona()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPersona, new NullStruct());
            }
            public Task<NullStruct> GetPersonaAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPersona, new NullStruct());
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
            
            public PersonaDetails DisablePersona(PersonaRequest request)
            {
                return Connection.SendRequest<PersonaRequest, PersonaDetails, NullStruct>(this, (ushort)AuthenticationComponentCommand.disablePersona, request);
            }
            public Task<PersonaDetails> DisablePersonaAsync(PersonaRequest request)
            {
                return Connection.SendRequestAsync<PersonaRequest, PersonaDetails, NullStruct>(this, (ushort)AuthenticationComponentCommand.disablePersona, request);
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
            
            public NullStruct XboxAssociateAccount()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxAssociateAccount, new NullStruct());
            }
            public Task<NullStruct> XboxAssociateAccountAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxAssociateAccount, new NullStruct());
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
            
            public SessionInfo Ps3AssociateAccount(ConsoleAssociateAccountRequest request)
            {
                return Connection.SendRequest<ConsoleAssociateAccountRequest, SessionInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3AssociateAccount, request);
            }
            public Task<SessionInfo> Ps3AssociateAccountAsync(ConsoleAssociateAccountRequest request)
            {
                return Connection.SendRequestAsync<ConsoleAssociateAccountRequest, SessionInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3AssociateAccount, request);
            }
            
            public ConsoleLoginResponse Ps3Login(PS3LoginRequest request)
            {
                return Connection.SendRequest<PS3LoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3Login, request);
            }
            public Task<ConsoleLoginResponse> Ps3LoginAsync(PS3LoginRequest request)
            {
                return Connection.SendRequestAsync<PS3LoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3Login, request);
            }
            
            public NullStruct ValidateSessionKey()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.validateSessionKey, new NullStruct());
            }
            public Task<NullStruct> ValidateSessionKeyAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.validateSessionKey, new NullStruct());
            }
            
            public NullStruct CreateWalUserSession()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.createWalUserSession, new NullStruct());
            }
            public Task<NullStruct> CreateWalUserSessionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.createWalUserSession, new NullStruct());
            }
            
            public ConsoleLoginResponse DeviceLoginGuest(DeviceLoginGuestRequest request)
            {
                return Connection.SendRequest<DeviceLoginGuestRequest, ConsoleLoginResponse, ConsoleCreateAccountRequest>(this, (ushort)AuthenticationComponentCommand.deviceLoginGuest, request);
            }
            public Task<ConsoleLoginResponse> DeviceLoginGuestAsync(DeviceLoginGuestRequest request)
            {
                return Connection.SendRequestAsync<DeviceLoginGuestRequest, ConsoleLoginResponse, ConsoleCreateAccountRequest>(this, (ushort)AuthenticationComponentCommand.deviceLoginGuest, request);
            }
            
            
            public override Type GetCommandRequestType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AuthenticationComponentCommand command) => AuthenticationComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AuthenticationComponentNotification notification) => AuthenticationComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<AuthenticationComponentCommand, AuthenticationComponentNotification, Blaze2RpcError>
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
            public virtual Task<NullStruct> UpdateAccountAsync(UpdateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateAccount, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.updateParentalEmail)]
            public virtual Task<NullStruct> UpdateParentalEmailAsync(UpdateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateAccountRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.updateParentalEmail, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getAccount)]
            public virtual Task<AccountInfo> GetAccountAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, AccountInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAccount, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.grantEntitlement)]
            public virtual Task<NullStruct> GrantEntitlementAsync(Entitlement request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<Entitlement, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.grantEntitlement, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getEntitlements)]
            public virtual Task<Entitlements> GetEntitlementsAsync(ListEntitlementsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ListEntitlementsRequest, Entitlements, NullStruct>(this, (ushort)AuthenticationComponentCommand.getEntitlements, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.hasEntitlement)]
            public virtual Task<NullStruct> HasEntitlementAsync(HasEntitlementRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<HasEntitlementRequest, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.hasEntitlement, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getUseCount)]
            public virtual Task<NullStruct> GetUseCountAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getUseCount, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.decrementUseCount)]
            public virtual Task<NullStruct> DecrementUseCountAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.decrementUseCount, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getAuthToken)]
            public virtual Task<NullStruct> GetAuthTokenAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getAuthToken, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.getHandoffToken)]
            public virtual Task<NullStruct> GetHandoffTokenAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getHandoffToken, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.listEntitlement2)]
            public virtual Task<NullStruct> ListEntitlement2Async(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.listEntitlement2, request);
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
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.expressLogin)]
            public virtual Task<FullLoginResponse> ExpressLoginAsync(ExpressLoginRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ExpressLoginRequest, FullLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.expressLogin, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.stressLogin)]
            public virtual Task<NullStruct> StressLoginAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.stressLogin, request);
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
            public virtual Task<NullStruct> GetPersonaAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.getPersona, request);
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
            public virtual Task<PersonaDetails> DisablePersonaAsync(PersonaRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<PersonaRequest, PersonaDetails, NullStruct>(this, (ushort)AuthenticationComponentCommand.disablePersona, request);
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
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.xboxAssociateAccount)]
            public virtual Task<NullStruct> XboxAssociateAccountAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.xboxAssociateAccount, request);
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
            public virtual Task<SessionInfo> Ps3AssociateAccountAsync(ConsoleAssociateAccountRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ConsoleAssociateAccountRequest, SessionInfo, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3AssociateAccount, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.ps3Login)]
            public virtual Task<ConsoleLoginResponse> Ps3LoginAsync(PS3LoginRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<PS3LoginRequest, ConsoleLoginResponse, NullStruct>(this, (ushort)AuthenticationComponentCommand.ps3Login, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.validateSessionKey)]
            public virtual Task<NullStruct> ValidateSessionKeyAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.validateSessionKey, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.createWalUserSession)]
            public virtual Task<NullStruct> CreateWalUserSessionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)AuthenticationComponentCommand.createWalUserSession, request);
            }
            
            [BlazeCommand((ushort)AuthenticationComponentCommand.deviceLoginGuest)]
            public virtual Task<ConsoleLoginResponse> DeviceLoginGuestAsync(DeviceLoginGuestRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<DeviceLoginGuestRequest, ConsoleLoginResponse, ConsoleCreateAccountRequest>(this, (ushort)AuthenticationComponentCommand.deviceLoginGuest, request);
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
            AuthenticationComponentCommand.getAccount => typeof(NullStruct),
            AuthenticationComponentCommand.grantEntitlement => typeof(Entitlement),
            AuthenticationComponentCommand.getEntitlements => typeof(ListEntitlementsRequest),
            AuthenticationComponentCommand.hasEntitlement => typeof(HasEntitlementRequest),
            AuthenticationComponentCommand.getUseCount => typeof(NullStruct),
            AuthenticationComponentCommand.decrementUseCount => typeof(NullStruct),
            AuthenticationComponentCommand.getAuthToken => typeof(NullStruct),
            AuthenticationComponentCommand.getHandoffToken => typeof(NullStruct),
            AuthenticationComponentCommand.listEntitlement2 => typeof(NullStruct),
            AuthenticationComponentCommand.login => typeof(LoginRequest),
            AuthenticationComponentCommand.acceptTos => typeof(AcceptTosRequest),
            AuthenticationComponentCommand.getTosInfo => typeof(GetTosInfoRequest),
            AuthenticationComponentCommand.consumecode => typeof(ConsumecodeRequest),
            AuthenticationComponentCommand.passwordForgot => typeof(PasswordForgotRequest),
            AuthenticationComponentCommand.silentLogin => typeof(SilentLoginRequest),
            AuthenticationComponentCommand.checkAgeReq => typeof(CheckAgeReqRequest),
            AuthenticationComponentCommand.expressLogin => typeof(ExpressLoginRequest),
            AuthenticationComponentCommand.stressLogin => typeof(NullStruct),
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
            AuthenticationComponentCommand.xboxAssociateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.xboxLogin => typeof(XboxLoginRequest),
            AuthenticationComponentCommand.ps3CreateAccount => typeof(ConsoleCreateAccountRequest),
            AuthenticationComponentCommand.ps3AssociateAccount => typeof(ConsoleAssociateAccountRequest),
            AuthenticationComponentCommand.ps3Login => typeof(PS3LoginRequest),
            AuthenticationComponentCommand.validateSessionKey => typeof(NullStruct),
            AuthenticationComponentCommand.createWalUserSession => typeof(NullStruct),
            AuthenticationComponentCommand.deviceLoginGuest => typeof(DeviceLoginGuestRequest),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(AuthenticationComponentCommand command) => command switch
        {
            AuthenticationComponentCommand.createAccount => typeof(CreateAccountResponse),
            AuthenticationComponentCommand.updateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.updateParentalEmail => typeof(NullStruct),
            AuthenticationComponentCommand.getAccount => typeof(AccountInfo),
            AuthenticationComponentCommand.grantEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.getEntitlements => typeof(Entitlements),
            AuthenticationComponentCommand.hasEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.getUseCount => typeof(NullStruct),
            AuthenticationComponentCommand.decrementUseCount => typeof(NullStruct),
            AuthenticationComponentCommand.getAuthToken => typeof(NullStruct),
            AuthenticationComponentCommand.getHandoffToken => typeof(NullStruct),
            AuthenticationComponentCommand.listEntitlement2 => typeof(NullStruct),
            AuthenticationComponentCommand.login => typeof(LoginResponse),
            AuthenticationComponentCommand.acceptTos => typeof(NullStruct),
            AuthenticationComponentCommand.getTosInfo => typeof(GetTosInfoResponse),
            AuthenticationComponentCommand.consumecode => typeof(ConsumecodeResponse),
            AuthenticationComponentCommand.passwordForgot => typeof(NullStruct),
            AuthenticationComponentCommand.silentLogin => typeof(FullLoginResponse),
            AuthenticationComponentCommand.checkAgeReq => typeof(CheckAgeReqResponse),
            AuthenticationComponentCommand.expressLogin => typeof(FullLoginResponse),
            AuthenticationComponentCommand.stressLogin => typeof(NullStruct),
            AuthenticationComponentCommand.logout => typeof(NullStruct),
            AuthenticationComponentCommand.createPersona => typeof(PersonaDetails),
            AuthenticationComponentCommand.getPersona => typeof(NullStruct),
            AuthenticationComponentCommand.listPersonas => typeof(ListPersonasResponse),
            AuthenticationComponentCommand.loginPersona => typeof(SessionInfo),
            AuthenticationComponentCommand.logoutPersona => typeof(NullStruct),
            AuthenticationComponentCommand.deletePersona => typeof(NullStruct),
            AuthenticationComponentCommand.disablePersona => typeof(PersonaDetails),
            AuthenticationComponentCommand.listDeviceAccounts => typeof(ListDeviceAccountsResponse),
            AuthenticationComponentCommand.xboxCreateAccount => typeof(ConsoleCreateAccountResponse),
            AuthenticationComponentCommand.xboxAssociateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.xboxLogin => typeof(ConsoleLoginResponse),
            AuthenticationComponentCommand.ps3CreateAccount => typeof(ConsoleCreateAccountResponse),
            AuthenticationComponentCommand.ps3AssociateAccount => typeof(SessionInfo),
            AuthenticationComponentCommand.ps3Login => typeof(ConsoleLoginResponse),
            AuthenticationComponentCommand.validateSessionKey => typeof(NullStruct),
            AuthenticationComponentCommand.createWalUserSession => typeof(NullStruct),
            AuthenticationComponentCommand.deviceLoginGuest => typeof(ConsoleLoginResponse),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(AuthenticationComponentCommand command) => command switch
        {
            AuthenticationComponentCommand.createAccount => typeof(FieldValidateErrorList),
            AuthenticationComponentCommand.updateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.updateParentalEmail => typeof(NullStruct),
            AuthenticationComponentCommand.getAccount => typeof(NullStruct),
            AuthenticationComponentCommand.grantEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.getEntitlements => typeof(NullStruct),
            AuthenticationComponentCommand.hasEntitlement => typeof(NullStruct),
            AuthenticationComponentCommand.getUseCount => typeof(NullStruct),
            AuthenticationComponentCommand.decrementUseCount => typeof(NullStruct),
            AuthenticationComponentCommand.getAuthToken => typeof(NullStruct),
            AuthenticationComponentCommand.getHandoffToken => typeof(NullStruct),
            AuthenticationComponentCommand.listEntitlement2 => typeof(NullStruct),
            AuthenticationComponentCommand.login => typeof(CreateAccountResponse),
            AuthenticationComponentCommand.acceptTos => typeof(NullStruct),
            AuthenticationComponentCommand.getTosInfo => typeof(NullStruct),
            AuthenticationComponentCommand.consumecode => typeof(NullStruct),
            AuthenticationComponentCommand.passwordForgot => typeof(NullStruct),
            AuthenticationComponentCommand.silentLogin => typeof(NullStruct),
            AuthenticationComponentCommand.checkAgeReq => typeof(FieldValidateErrorList),
            AuthenticationComponentCommand.expressLogin => typeof(NullStruct),
            AuthenticationComponentCommand.stressLogin => typeof(NullStruct),
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
            AuthenticationComponentCommand.xboxAssociateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.xboxLogin => typeof(NullStruct),
            AuthenticationComponentCommand.ps3CreateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.ps3AssociateAccount => typeof(NullStruct),
            AuthenticationComponentCommand.ps3Login => typeof(NullStruct),
            AuthenticationComponentCommand.validateSessionKey => typeof(NullStruct),
            AuthenticationComponentCommand.createWalUserSession => typeof(NullStruct),
            AuthenticationComponentCommand.deviceLoginGuest => typeof(ConsoleCreateAccountRequest),
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
            getAccount = 30,
            grantEntitlement = 31,
            getEntitlements = 32,
            hasEntitlement = 33,
            getUseCount = 34,
            decrementUseCount = 35,
            getAuthToken = 36,
            getHandoffToken = 37,
            listEntitlement2 = 38,
            login = 40,
            acceptTos = 41,
            getTosInfo = 42,
            consumecode = 44,
            passwordForgot = 45,
            silentLogin = 50,
            checkAgeReq = 51,
            expressLogin = 60,
            stressLogin = 61,
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
            xboxAssociateAccount = 160,
            xboxLogin = 170,
            ps3CreateAccount = 180,
            ps3AssociateAccount = 190,
            ps3Login = 200,
            validateSessionKey = 210,
            createWalUserSession = 230,
            deviceLoginGuest = 300,
        }
        
        public enum AuthenticationComponentNotification : ushort
        {
        }
        
    }
}
