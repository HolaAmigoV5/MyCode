// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

#pragma once

#include "stdafx.h"
#include <string>
#include "cpprest/details/basic_types.h"
#include "cpprest/asyncrt_utils.h"
#include <chrono>

std::string url;

void get_url(int argc, char* argv[])
{
    url = "http://localhost:42524/";

    for (int i = 0; i < argc; ++i)
    {
        std::string str = argv[i];

        auto pos = str.find("url=");
        if (pos != std::string::npos)
        {
            url = str.substr(pos + 4);
            break;
        }
    }
}
