#pragma once
#include <msclr\marshal_cppstd.h>
// #include <msclr\marshal.h>
using namespace System;

namespace OpenCV
{
    public ref class Img sealed
    {
    public:
        void showImg(String^ path);
        void showImgAlter(String^ path);

        void showImgDefault(String^ path);

        String^ getProjectPath();
    };

    /*public class my_class final
    {
    public:
    };*/
}
