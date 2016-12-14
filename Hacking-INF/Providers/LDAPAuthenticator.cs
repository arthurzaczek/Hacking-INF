using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Web;

namespace Hacking_INF.Providers
{
    public class LDAPUser
    {
        public LDAPUser()
        {
            this.IsAuthenticated = false;
        }

        public LDAPUser(bool auth, string firstname, string lastname, string fullname, string email, string studiengang, string studiengangKuerzel, string personal)
        {
            this.IsAuthenticated = auth;
            this.Firstname = firstname;
            this.Lastname = lastname;
            this.Fullname = fullname;
            this.Mail = email;
            this.Studiengang = studiengang;
            this.StudiengangKuerzel = studiengangKuerzel;
            this.PersonalType = personal;
        }

        public bool IsAuthenticated { get; private set; }

        public string Mail { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
        public string Fullname { get; private set; }
        public string Studiengang { get; private set; }
        public string StudiengangKuerzel { get; private set; }
        public string PersonalType { get; private set; }
    }

    public class LDAPAuthenticator
    {
        const string ATTRIBUTES = "ou=People,dc=technikum-wien,dc=at";
        const string HOST = "ldap.technikum-wien.at";
        const int PORT = 389;
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(LDAPAuthenticator));

        private static bool? _disablePasswordCheck;
        public static bool DisablePasswordCheck
        {
            get
            {
                if (_disablePasswordCheck == null)
                {
                    // set DEV_ENVIRONMENT = 1, done by IISExpress
                    _disablePasswordCheck = System.Environment.GetEnvironmentVariable("DEV_ENVIRONMENT") == "1";
                    _log.InfoFormat("DisablePasswordCheck = {0}", _disablePasswordCheck);
                }
                return _disablePasswordCheck.Value;
            }
        }

        private static bool? _startTLS;
        public static bool StartTLS
        {
            get
            {
                if (_startTLS == null)
                {
                    _startTLS = (System.Configuration.ConfigurationManager.AppSettings["ldap_start_tls"] as string ?? "true") == "true";
                    _log.InfoFormat("StartTLS = {0}", _startTLS);
                }
                return _startTLS.Value;
            }
        }

        private static LDAPUser GetUserParameter(LdapConnection ldap, string username)
        {
            try
            {
                ldap.Bind();

                //user daten parameter lesen - frage: daten nur holen, wenn noch nicht registriert oder sollen die daten immer verwendet werden und u.U auch in sofi aktualisiert werden
                string[] attributesToReturn = new string[] { "displayName", "sn", "givenName", "cn", "mail", "ou", "gidNumber", "uid" };
                SearchRequest searchRequest = new SearchRequest(ATTRIBUTES, string.Format("(uid={0})", username), System.DirectoryServices.Protocols.SearchScope.Subtree, null);

                SearchResponse searchResponse = (SearchResponse)ldap.SendRequest(searchRequest);
                if (searchResponse.Entries.Count != 1)
                {
                    return new LDAPUser();
                }

                SearchResultEntry entry = searchResponse.Entries[0];

                var result = new LDAPUser();
                string sMail = "";
                string sDisplayName = "";
                string sFirstName = "";
                string sLastName = "";
                string sStudiengang = "";
                string sStudiengangKuerzel = "";
                string sPersonalBezeichnung = "";

                try
                {
                    sMail = (string)entry.Attributes["mail"]?[0] ?? string.Empty;
                    sLastName = (string)entry.Attributes["sn"]?[0] ?? string.Empty;
                    sFirstName = (string)entry.Attributes["givenName"]?[0] ?? string.Empty;
                    sDisplayName = (string)entry.Attributes["displayName"]?[0] ?? string.Format("{0} {1}", sFirstName, sLastName);
                    string iGidNumber = (string)entry.Attributes["gidNumber"]?[0] ?? string.Empty; // 101=Technikum, 102=Student

                    sStudiengang = "";
                    sStudiengangKuerzel = "";
                    sPersonalBezeichnung = "";
                    DirectoryAttribute oOu = entry.Attributes["ou"];
                    if (oOu.Count > 0 && iGidNumber == "101") //tw-personal - in ou steht meist Teacher
                    {
                        //sStudiengang = (string)oOu[1];
                        sPersonalBezeichnung = (string)oOu[1];
                    }
                    else if (oOu.Count > 0 && iGidNumber == "102") //student
                    {
                        sStudiengang = (string)oOu[1];
                        sStudiengangKuerzel = (string)oOu[2];
                    }

                }
                catch (Exception ex)
                {
                    _log.Warn("Unable to get ldap user attributes.", ex);
                }


                return new LDAPUser(true, sFirstName, sLastName, sDisplayName, sMail, sStudiengang, sStudiengangKuerzel, sPersonalBezeichnung);
            }
            catch (Exception ex)
            {
                _log.Warn("Unable to get ldap user, might be a auth fail.", ex);
                return new LDAPUser();
            }
        }

        public static LDAPUser GetUserParameter(string username)
        {
            using (var ldap = Connect())
            {
                ldap.AuthType = AuthType.Anonymous;

                return GetUserParameter(ldap, username);
            }
        }

        public static LDAPUser Authenticate(string username, string password)
        {
            using (var ldap = Connect())
            {

                if (DisablePasswordCheck == false)
                {
                    NetworkCredential credential = new NetworkCredential();
                    credential.UserName = "uid=" + username + "," + ATTRIBUTES;
                    credential.Password = password;

                    ldap.AuthType = AuthType.Basic;
                    ldap.Credential = credential;
                }
                else
                {
                    ldap.AuthType = AuthType.Anonymous;
                }

                return GetUserParameter(ldap, username);
            }
        }

        private static LdapConnection Connect()
        {
            var ldap = new LdapConnection(HOST + ":" + PORT.ToString());
            if (StartTLS)
            {
                ldap.SessionOptions.ProtocolVersion = 3;
                ldap.SessionOptions.StartTransportLayerSecurity(null);
            }
            return ldap;
        }
    }
}