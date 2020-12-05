// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using BasicTestApp;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure;
using Microsoft.AspNetCore.Components.E2ETest.Infrastructure.ServerFixtures;
using Microsoft.AspNetCore.E2ETesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.AspNetCore.Components.E2ETest.Tests
{
    public class BindTest : BasicTestAppTestBase
    {
        public BindTest(
            BrowserFixture browserFixture,
            ToggleExecutionModeServerFixture<Program> serverFixture,
            ITestOutputHelper output)
            : base(browserFixture, serverFixture, output)
        {
        }

        protected override void InitializeAsyncCore()
        {
            // On WebAssembly, page reloads are expensive so skip if possible
            Navigate(ServerPathBase, noReload: !_serverFixture.UsingAspNetHost);
            MountTestComponent<BindCasesComponent>();
        }

        [Fact]
        public void CanBindTextbox_InitiallyBlank()
        {
            var target = Browser.FindElement(By.Id("textbox-initially-blank"));
            var boundValue = Browser.FindElement(By.Id("textbox-initially-blank-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-initially-blank-mirror"));
            var setNullButton = Browser.FindElement(By.Id("textbox-initially-blank-setnull"));
            Assert.Equal(string.Empty, target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("Changed value");
            Assert.Equal(string.Empty, boundValue.Text); // Doesn't update until change event
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));
            target.SendKeys("\t");
            Browser.Equal("Changed value", () => boundValue.Text);
            Assert.Equal("Changed value", mirrorValue.GetAttribute("value"));

            // Remove the value altogether
            setNullButton.Click();
            Browser.Equal(string.Empty, () => target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextbox_InitiallyPopulated()
        {
            var target = Browser.FindElement(By.Id("textbox-initially-populated"));
            var boundValue = Browser.FindElement(By.Id("textbox-initially-populated-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-initially-populated-mirror"));
            var setNullButton = Browser.FindElement(By.Id("textbox-initially-populated-setnull"));
            Assert.Equal("Hello", target.GetAttribute("value"));
            Assert.Equal("Hello", boundValue.Text);
            Assert.Equal("Hello", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("Changed value\t");
            Browser.Equal("Changed value", () => boundValue.Text);
            Assert.Equal("Changed value", mirrorValue.GetAttribute("value"));

            // Remove the value altogether
            setNullButton.Click();
            Browser.Equal(string.Empty, () => target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextArea_InitiallyBlank()
        {
            var target = Browser.FindElement(By.Id("textarea-initially-blank"));
            var boundValue = Browser.FindElement(By.Id("textarea-initially-blank-value"));
            Assert.Equal(string.Empty, target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);

            // Modify target; verify value is updated
            target.SendKeys("Changed value");
            Assert.Equal(string.Empty, boundValue.Text); // Don't update as there's no change event fired yet.
            target.SendKeys("\t");
            Browser.Equal("Changed value", () => boundValue.Text);
        }

        [Fact]
        public void CanBindTextArea_InitiallyPopulated()
        {
            var target = Browser.FindElement(By.Id("textarea-initially-populated"));
            var boundValue = Browser.FindElement(By.Id("textarea-initially-populated-value"));
            Assert.Equal("Hello", target.GetAttribute("value"));
            Assert.Equal("Hello", boundValue.Text);

            // Modify target; verify value is updated
            target.Clear();
            target.SendKeys("Changed value\t");
            Browser.Equal("Changed value", () => boundValue.Text);
        }

        [Fact]
        public void CanBindCheckbox_InitiallyNull()
        {
            var target = Browser.FindElement(By.Id("checkbox-initially-null"));
            var boundValue = Browser.FindElement(By.Id("checkbox-initially-null-value"));
            var invertButton = Browser.FindElement(By.Id("checkbox-initially-null-invert"));
            Assert.False(target.Selected);
            Assert.Equal(string.Empty, boundValue.Text);

            // Modify target; verify value is updated
            target.Click();
            Browser.True(() => target.Selected);
            Browser.Equal("True", () => boundValue.Text);

            // Modify data; verify checkbox is updated
            invertButton.Click();
            Browser.False(() => target.Selected);
            Browser.Equal("False", () => boundValue.Text);
        }

        [Fact]
        public void CanBindCheckbox_InitiallyUnchecked()
        {
            var target = Browser.FindElement(By.Id("checkbox-initially-unchecked"));
            var boundValue = Browser.FindElement(By.Id("checkbox-initially-unchecked-value"));
            var invertButton = Browser.FindElement(By.Id("checkbox-initially-unchecked-invert"));
            Assert.False(target.Selected);
            Assert.Equal("False", boundValue.Text);

            // Modify target; verify value is updated
            target.Click();
            Browser.True(() => target.Selected);
            Browser.Equal("True", () => boundValue.Text);

            // Modify data; verify checkbox is updated
            invertButton.Click();
            Browser.False(() => target.Selected);
            Browser.Equal("False", () => boundValue.Text);
        }

        [Fact]
        public void CanBindCheckbox_InitiallyChecked()
        {
            var target = Browser.FindElement(By.Id("checkbox-initially-checked"));
            var boundValue = Browser.FindElement(By.Id("checkbox-initially-checked-value"));
            var invertButton = Browser.FindElement(By.Id("checkbox-initially-checked-invert"));
            Assert.True(target.Selected);
            Assert.Equal("True", boundValue.Text);

            // Modify target; verify value is updated
            target.Click();
            Browser.False(() => target.Selected);
            Browser.Equal("False", () => boundValue.Text);

            // Modify data; verify checkbox is updated
            invertButton.Click();
            Browser.True(() => target.Selected);
            Browser.Equal("True", () => boundValue.Text);
        }

        [Fact]
        public void CanBindSelect()
        {
            var target = new SelectElement(Browser.FindElement(By.Id("select-box")));
            var boundValue = Browser.FindElement(By.Id("select-box-value"));
            Assert.Equal("Second choice", target.SelectedOption.Text);
            Assert.Equal("Second", boundValue.Text);

            // Modify target; verify value is updated
            target.SelectByText("Third choice");
            Browser.Equal("Third", () => boundValue.Text);

            // Also verify we can add and select new options atomically
            // Don't move this into a separate test, because then the previous assertions
            // would be dependent on test execution order (or would require a full page reload)
            Browser.FindElement(By.Id("select-box-add-option")).Click();
            Browser.Equal("Fourth", () => boundValue.Text);
            Assert.Equal("Fourth choice", target.SelectedOption.Text);
        }

        [Fact]
        public void CanBindTextboxInt()
        {
            var target = Browser.FindElement(By.Id("textbox-int"));
            var boundValue = Browser.FindElement(By.Id("textbox-int-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-int-mirror"));
            Assert.Equal("-42", target.GetAttribute("value"));
            Assert.Equal("-42", boundValue.Text);
            Assert.Equal("-42", mirrorValue.GetAttribute("value"));

            // Modify target; value is not updated because it's not convertable.
            target.Clear();
            Browser.Equal("-42", () => boundValue.Text);
            Assert.Equal("-42", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("42\t");
            Browser.Equal("42", () => boundValue.Text);
            Assert.Equal("42", mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxNullableInt()
        {
            var target = Browser.FindElement(By.Id("textbox-nullable-int"));
            var boundValue = Browser.FindElement(By.Id("textbox-nullable-int-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-nullable-int-mirror"));
            Assert.Equal(string.Empty, target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            Browser.Equal("", () => boundValue.Text);
            Assert.Equal("", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("-42\t");
            Browser.Equal("-42", () => boundValue.Text);
            Assert.Equal("-42", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("42\t");
            Browser.Equal("42", () => boundValue.Text);
            Assert.Equal("42", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("\t");
            Browser.Equal(string.Empty, () => boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxLong()
        {
            var target = Browser.FindElement(By.Id("textbox-long"));
            var boundValue = Browser.FindElement(By.Id("textbox-long-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-long-mirror"));
            Assert.Equal("3000000000", target.GetAttribute("value"));
            Assert.Equal("3000000000", boundValue.Text);
            Assert.Equal("3000000000", mirrorValue.GetAttribute("value"));

            // Modify target; value is not updated because it's not convertable.
            target.Clear();
            Browser.Equal("3000000000", () => boundValue.Text);
            Assert.Equal("3000000000", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("-3000000000\t");
            Browser.Equal("-3000000000", () => boundValue.Text);
            Assert.Equal("-3000000000", mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxNullableLong()
        {
            var target = Browser.FindElement(By.Id("textbox-nullable-long"));
            var boundValue = Browser.FindElement(By.Id("textbox-nullable-long-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-nullable-long-mirror"));
            Assert.Equal(string.Empty, target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            Browser.Equal("", () => boundValue.Text);
            Assert.Equal("", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("3000000000\t");
            Browser.Equal("3000000000", () => boundValue.Text);
            Assert.Equal("3000000000", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("-3000000000\t");
            Browser.Equal("-3000000000", () => boundValue.Text);
            Assert.Equal("-3000000000", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("\t");
            Browser.Equal(string.Empty, () => boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxFloat()
        {
            var target = Browser.FindElement(By.Id("textbox-float"));
            var boundValue = Browser.FindElement(By.Id("textbox-float-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-float-mirror"));
            Assert.Equal("3.141", target.GetAttribute("value"));
            Assert.Equal("3.141", boundValue.Text);
            Assert.Equal("3.141", mirrorValue.GetAttribute("value"));

            // Modify target; value is not updated because it's not convertable.
            target.Clear();
            Browser.Equal("3.141", () => boundValue.Text);
            Assert.Equal("3.141", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("-3.141\t");
            Browser.Equal("-3.141", () => boundValue.Text);
            Assert.Equal("-3.141", mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxNullableFloat()
        {
            var target = Browser.FindElement(By.Id("textbox-nullable-float"));
            var boundValue = Browser.FindElement(By.Id("textbox-nullable-float-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-nullable-float-mirror"));
            Assert.Equal(string.Empty, target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            Browser.Equal("", () => boundValue.Text);
            Assert.Equal("", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("3.141\t");
            Browser.Equal("3.141", () => boundValue.Text);
            Assert.Equal("3.141", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("-3.141\t");
            Browser.Equal("-3.141", () => boundValue.Text);
            Assert.Equal("-3.141", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("\t");
            Browser.Equal(string.Empty, () => boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxDouble()
        {
            var target = Browser.FindElement(By.Id("textbox-double"));
            var boundValue = Browser.FindElement(By.Id("textbox-double-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-double-mirror"));
            Assert.Equal("3.14159265359", target.GetAttribute("value"));
            Assert.Equal("3.14159265359", boundValue.Text);
            Assert.Equal("3.14159265359", mirrorValue.GetAttribute("value"));

            // Modify target; value is not updated because it's not convertable.
            target.Clear();
            Browser.Equal("3.14159265359", () => boundValue.Text);
            Assert.Equal("3.14159265359", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("-3.14159265359\t");
            Browser.Equal("-3.14159265359", () => boundValue.Text);
            Assert.Equal("-3.14159265359", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            // Double shouldn't preserve trailing zeros
            target.Clear();
            target.SendKeys("0.010\t");
            Browser.Equal("0.01", () => boundValue.Text);
            Assert.Equal("0.01", mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxNullableDouble()
        {
            var target = Browser.FindElement(By.Id("textbox-nullable-double"));
            var boundValue = Browser.FindElement(By.Id("textbox-nullable-double-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-nullable-double-mirror"));
            Assert.Equal(string.Empty, target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            Browser.Equal("", () => boundValue.Text);
            Assert.Equal("", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("3.14159265359\t");
            Browser.Equal("3.14159265359", () => boundValue.Text);
            Assert.Equal("3.14159265359", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("-3.14159265359\t");
            Browser.Equal("-3.14159265359", () => boundValue.Text);
            Assert.Equal("-3.14159265359", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            // Double shouldn't preserve trailing zeros
            target.Clear();
            target.SendKeys("0.010\t");
            Browser.Equal("0.01", () => boundValue.Text);
            Assert.Equal("0.01", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("\t");
            Browser.Equal(string.Empty, () => boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxDecimal()
        {
            var target = Browser.FindElement(By.Id("textbox-decimal"));
            var boundValue = Browser.FindElement(By.Id("textbox-decimal-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-decimal-mirror"));
            Assert.Equal("0.0000000000000000000000000001", target.GetAttribute("value"));
            Assert.Equal("0.0000000000000000000000000001", boundValue.Text);
            Assert.Equal("0.0000000000000000000000000001", mirrorValue.GetAttribute("value"));

            // Modify target; value is not updated because it's not convertable.
            target.Clear();
            Browser.Equal("0.0000000000000000000000000001", () => boundValue.Text);
            Assert.Equal("0.0000000000000000000000000001", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            // Decimal should preserve trailing zeros
            target.SendKeys("0.010\t");
            Browser.Equal("0.010", () => boundValue.Text);
            Assert.Equal("0.010", mirrorValue.GetAttribute("value"));
        }

        [Fact]
        public void CanBindTextboxNullableDecimal()
        {
            var target = Browser.FindElement(By.Id("textbox-nullable-decimal"));
            var boundValue = Browser.FindElement(By.Id("textbox-nullable-decimal-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-nullable-decimal-mirror"));
            Assert.Equal(string.Empty, target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            Browser.Equal("", () => boundValue.Text);
            Assert.Equal("", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.SendKeys("0.0000000000000000000000000001\t");
            Browser.Equal("0.0000000000000000000000000001", () => boundValue.Text);
            Assert.Equal("0.0000000000000000000000000001", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            // Decimal should preserve trailing zeros
            target.Clear();
            target.SendKeys("0.010\t");
            Browser.Equal("0.010", () => boundValue.Text);
            Assert.Equal("0.010", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("\t");
            Browser.Equal(string.Empty, () => boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));
        }

        // This tests what happens you put invalid (unconvertable) input in. This is separate from the
        // other tests because it requires type="text" - the other tests use type="number"
        [Fact]
        public void CanBindTextbox_Decimal_InvalidInput()
        {
            var target = Browser.FindElement(By.Id("textbox-decimal-invalid"));
            var boundValue = Browser.FindElement(By.Id("textbox-decimal-invalid-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-decimal-invalid-mirror"));
            Assert.Equal("0.0000000000000000000000000001", target.GetAttribute("value"));
            Assert.Equal("0.0000000000000000000000000001", boundValue.Text);
            Assert.Equal("0.0000000000000000000000000001", mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("0.01\t");
            Browser.Equal("0.01", () => boundValue.Text);
            Assert.Equal("0.01", mirrorValue.GetAttribute("value"));

            // Modify target to something invalid - the invalid value is preserved in the input, the other displays
            // don't change and still have the last value valid.
            target.SendKeys("A\t");
            Browser.Equal("0.01", () => boundValue.Text);
            Assert.Equal("0.01", mirrorValue.GetAttribute("value"));
            Assert.Equal("0.01A", target.GetAttribute("value"));

            // Modify target to something valid.
            target.SendKeys(Keys.Backspace);
            target.SendKeys("1\t");
            Browser.Equal("0.011", () => boundValue.Text);
            Assert.Equal("0.011", mirrorValue.GetAttribute("value"));
        }

        // This tests what happens you put invalid (unconvertable) input in. This is separate from the
        // other tests because it requires type="text" - the other tests use type="number"
        [Fact]
        public void CanBindTextbox_NullableDecimal_InvalidInput()
        {
            var target = Browser.FindElement(By.Id("textbox-nullable-decimal-invalid"));
            var boundValue = Browser.FindElement(By.Id("textbox-nullable-decimal-invalid-value"));
            var mirrorValue = Browser.FindElement(By.Id("textbox-nullable-decimal-invalid-mirror"));
            Assert.Equal(string.Empty, target.GetAttribute("value"));
            Assert.Equal(string.Empty, boundValue.Text);
            Assert.Equal(string.Empty, mirrorValue.GetAttribute("value"));

            // Modify target; verify value is updated and that textboxes linked to the same data are updated
            target.Clear();
            target.SendKeys("0.01\t");
            Browser.Equal("0.01", () => boundValue.Text);
            Assert.Equal("0.01", mirrorValue.GetAttribute("value"));

            // Modify target to something invalid - the invalid value is preserved in the input, the other displays
            // don't change and still have the last value valid.
            target.SendKeys("A\t");
            Browser.Equal("0.01", () => boundValue.Text);
            Assert.Equal("0.01", mirrorValue.GetAttribute("value"));
            Assert.Equal("0.01A", target.GetAttribute("value"));

            // Modify target to something valid.
            target.SendKeys(Keys.Backspace);
            target.SendKeys("1\t");
            Browser.Equal("0.011", () => boundValue.Text);
            Assert.Equal("0.011", mirrorValue.GetAttribute("value"));
        }
    }
}
