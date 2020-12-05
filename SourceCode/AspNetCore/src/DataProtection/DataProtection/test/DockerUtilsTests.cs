﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace Microsoft.AspNetCore.DataProtection.Test
{
    public class DockerUtilsTests
    {
        // example of content from /proc/self/mounts
        private static readonly string[] fstab = new []
        {
            "none / aufs rw,relatime,si=f9bfcf896de3f6c2,dio,dirperm1 0 0",
            "# comments",
            "",
            "proc /proc proc rw,nosuid,nodev,noexec,relatime 0 0",
            "tmpfs /dev tmpfs rw,nosuid,mode=755 0 0",
            "devpts /dev/pts devpts rw,nosuid,noexec,relatime,gid=5,mode=620,ptmxmode=666 0 0",
            "/dev/vda2 /etc/resolv.conf ext4 rw,relatime,data=ordered 0 0",
            "/dev/vda2 /etc/hostname ext4 rw,relatime,data=ordered 0 0",
            "/dev/vda2 /etc/hosts ext4 rw,relatime,data=ordered 0 0",
            "shm /dev/shm tmpfs rw,nosuid,nodev,noexec,relatime,size=65536k 0 0",
            // the mounted directory
            "osxfs /app fuse.osxfs rw,nosuid,nodev,relatime,user_id=0,group_id=0,allow_other,max_read=1048576 0 0",
        };

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Windows)]
        [InlineData("/")]
        [InlineData("/home")]
        [InlineData("/home/")]
        [InlineData("/home/root")]
        [InlineData("./dir")]
        [InlineData("../dir")]
        public void DeterminesFolderIsNotMounted(string directory)
        {
            Assert.False(DockerUtils.IsDirectoryMounted(new DirectoryInfo(directory), fstab));
        }

        [ConditionalTheory]
        [OSSkipCondition(OperatingSystems.Windows)]
        [InlineData("/app")]
        [InlineData("/app/")]
        [InlineData("/app/subdir")]
        [InlineData("/app/subdir/")]
        [InlineData("/app/subdir/two")]
        [InlineData("/app/subdir/two/")]
        public void DeterminesFolderIsMounted(string directory)
        {
            Assert.True(DockerUtils.IsDirectoryMounted(new DirectoryInfo(directory), fstab));
        }
    }
}
