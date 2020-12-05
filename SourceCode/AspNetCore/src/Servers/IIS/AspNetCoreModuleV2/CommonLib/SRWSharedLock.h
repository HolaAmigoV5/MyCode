// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

#pragma once

#include <synchapi.h>

class SRWSharedLock
{
public:
	SRWSharedLock(const SRWLOCK& lock);
	~SRWSharedLock();
private:
    const SRWLOCK& m_lock;
};
