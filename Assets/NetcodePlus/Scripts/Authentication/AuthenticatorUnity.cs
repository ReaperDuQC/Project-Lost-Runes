using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace NetcodePlus
{
    /// <summary>
    /// This authenticator is the base auth for Unity Services
    /// It will login in anonymous mode
    /// It is ideal for quick testing since it will skip login UI and create a temporary user.
    /// </summary>

    public class AuthenticatorUnity : Authenticator
    {
        public override async Task Initialize()
        {
            await base.Initialize();
            if(UnityServices.State == ServicesInitializationState.Uninitialized)
                await UnityServices.InitializeAsync();
        }

        public override async Task<bool> Login()
        {
            if (IsConnected())
                return true; //Already connected

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                user_id = AuthenticationService.Instance.PlayerId;
                if (username == null)
                    username = user_id;
                return true;
            }
            catch (AuthenticationException ex) { Debug.LogException(ex); }
            catch (RequestFailedException ex) { Debug.LogException(ex); }
            return false;
        }

        public override async Task<bool> Login(string username)
        {
            this.username = username;
            return await Login();
        }

        public override void Logout()
        {
            try
            {
                AuthenticationService.Instance.SignOut(true);
                user_id = null;
                username = null;
            }
            catch (System.Exception) { }
        }

        public override bool IsConnected()
        {
            return AuthenticationService.Instance.IsAuthorized;
        }

        public override bool IsSignedIn()
        {
            return AuthenticationService.Instance.IsSignedIn;
        }

        public override bool IsExpired()
        {
            return AuthenticationService.Instance.IsExpired;
        }

        public override bool IsUnityServices()
        {
            return true;
        }

        public override string GetUsername()
        {
            return username;
        }

        public override string GetUserId()
        {
            return user_id;
        }
    }
}
