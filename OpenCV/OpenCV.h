#pragma once
#include <msclr\marshal_cppstd.h>
using namespace System;

namespace OpenCV
{
    public ref class Img sealed
    {
    public:
        void showImg(String^ path);

        String^ getProjectPath();
    };

    /*public class my_class final
    {
    public:
    };*/
}
