// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Xunit;

namespace Microsoft.AspNetCore.Routing.Constraints
{
    public class NonFileNameRouteConstraintTest
    {
        [Theory]
        [MemberData(nameof(FileNameRouteConstraintTest.FileNameData), MemberType = typeof(FileNameRouteConstraintTest))]
        public void Match_RouteValue_IsNotNonFileName(object value)
        {
            // Arrange
            var constraint = new NonFileNameRouteConstraint();

            var values = new RouteValueDictionary();
            values.Add("path", value);

            // Act
            var result = constraint.Match(httpContext: null, route: null, "path", values, RouteDirection.IncomingRequest);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [MemberData(nameof(FileNameRouteConstraintTest.NonFileNameData), MemberType = typeof(FileNameRouteConstraintTest))]
        public void Match_RouteValue_IsNonFileName(object value)
        {
            // Arrange
            var constraint = new NonFileNameRouteConstraint();

            var values = new RouteValueDictionary();
            values.Add("path", value);

            // Act
            var result = constraint.Match(httpContext: null, route: null, "path", values, RouteDirection.IncomingRequest);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Match_MissingValue_IsNotFileName()
        {
            // Arrange
            var constraint = new NonFileNameRouteConstraint();

            var values = new RouteValueDictionary();

            // Act
            var result = constraint.Match(httpContext: null, route: null, "path", values, RouteDirection.IncomingRequest);

            // Assert
            Assert.True(result);
        }
    }
}
