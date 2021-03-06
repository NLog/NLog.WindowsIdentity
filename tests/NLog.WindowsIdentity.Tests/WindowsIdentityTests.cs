// 
// Copyright (c) 2004-2021 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF 
// THE POSSIBILITY OF SUCH DAMAGE.
// 

namespace NLog.WindowsIdentity.Tests
{
    using System;
    using Xunit;

    public class WindowsIdentityTests
    {
        public WindowsIdentityTests()
        {
            LogManager.ThrowExceptions = true;
        }

        [Fact]
        public void WindowsIdentityTest()
        {
            var userDomain = Environment.GetEnvironmentVariable("USERDOMAIN") ?? string.Empty;
            var userName = Environment.GetEnvironmentVariable("USERNAME") ?? string.Empty;
            var fullName = !string.IsNullOrEmpty(userDomain) ? (userDomain + "\\" + userName) : userName;

            var logFactory = new LogFactory().Setup()
                .SetupExtensions(ext => ext.RegisterAssembly(typeof(NLog.LayoutRenderers.WindowsIdentityLayoutRenderer).Assembly))
                .LoadConfigurationFromXml(@"
                <nlog>
                    <targets><target type='debug' name='debug' layout='${windows-identity} - ${windows-identity:domain=false} - ${windows-identity:username=false}' /></targets>
                    <rules><logger name='*' writeTo='debug' /></rules>
                </nlog>").LogFactory;

            var debugTarget = logFactory.Configuration.FindTargetByName<NLog.Targets.DebugTarget>("debug");
            logFactory.GetCurrentClassLogger().Info("Test Message");
            Assert.Equal(1, debugTarget.Counter);
#if !DEBUG
            if (!string.IsNullOrEmpty(debugTarget.LastMessage))
#endif
                Assert.Equal($"{fullName} - {userName} - {userDomain}", debugTarget.LastMessage);
        }
    }
}
