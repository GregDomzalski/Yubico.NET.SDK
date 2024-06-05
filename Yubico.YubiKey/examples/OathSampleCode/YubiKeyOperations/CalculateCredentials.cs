// Copyright 2021 Yubico AB
//
// Licensed under the Apache License, Version 2.0 (the "License").
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Yubico.YubiKey.Oath;
using Yubico.YubiKey.Sample.SharedCode;

namespace Yubico.YubiKey.Sample.OathSampleCode
{
    public static class CalculateCredentials
    {
        // Get OTP (One-Time Password) values for all configured credentials on the YubiKey.
        public static bool RunCalculateCredentials(IYubiKeyDevice yubiKey, Func<KeyEntryData, bool> KeyCollectorDelegate)
        {
            using var oathSession = new OathSession(yubiKey);
            {
                oathSession.KeyCollector = KeyCollectorDelegate;
                IDictionary<Credential, Code> result = oathSession.CalculateAllCredentials();
                ReportAllResults(result);
            }

            return true;
        }

        // Get an OTP (One-Time Password) value for the specific credential on the YubiKey.
        public static bool RunCalculateOneCredential(
            IYubiKeyDevice yubiKey,
            Credential credential,
            Func<KeyEntryData, bool> KeyCollectorDelegate)
        {
            if (credential is null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            using var oathSession = new OathSession(yubiKey);
            {
                oathSession.KeyCollector = KeyCollectorDelegate;
                Code code = oathSession.CalculateCredential(credential);
                ReportOneResult(credential, code);
            }

            return true;
        }

        private static void ReportAllResults(IDictionary<Credential, Code> credentials)
        {
            // Are there any?
            var outputList = new StringBuilder("");
            if (credentials.Count > 0)
            {
                _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Number of credentials: {credentials.Count}");
                _ = outputList.AppendLine();
                foreach (KeyValuePair<Credential, Code> pair in credentials)
                {
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Issuer    : {pair.Key.Issuer}");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Account   : {pair.Key.AccountName}");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Type      : {pair.Key.Type}");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Period    : {(int?)pair.Key.Period}sec");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Digits    : {pair.Key.Digits}");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Touch     : {pair.Key.RequiresTouch}");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"OTP code  : {pair.Value.Value}");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"ValidFrom : {pair.Value.ValidFrom}");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"ValidUntil: {pair.Value.ValidUntil}");
                    _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Name      : {pair.Key.Name}");
                    _ = outputList.AppendLine();
                }
            }
            else
            {
                _ = outputList.AppendLine("No credentials on this YubiKey");
            }

            SampleMenu.WriteMessage(MessageType.Special, 0, outputList.ToString());
        }

        private static void ReportOneResult(Credential credential, Code code)
        {
            var outputList = new StringBuilder("Calculated credential:");
            _ = outputList.AppendLine();
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Issuer    : {credential.Issuer}");
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Account   : {credential.AccountName}");
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Type      : {credential.Type}");
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Period    : {(int?)credential.Period}sec");
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Digits    : {credential.Digits}");
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"OTP code  : {code.Value}");
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"ValidFrom : {code.ValidFrom}");
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"ValidUntil: {code.ValidUntil}");
            _ = outputList.AppendLine(CultureInfo.InvariantCulture, $"Name      : {credential.Name}");
            _ = outputList.AppendLine();

            SampleMenu.WriteMessage(MessageType.Special, 0, outputList.ToString());
        }
    }
}
