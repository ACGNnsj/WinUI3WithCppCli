#include "pch.h"
#include "OpenCV.h"
#pragma unmanaged
// #undef _M_CEE
#include <opencv2/opencv.hpp>
// #define _M_CEE
#pragma managed
#include <direct.h>
using namespace OpenCV;

void Img::showImg(String^ path)
{
    cli::array<wchar_t>^ chArray = path->ToCharArray();
    cli::array<unsigned char, 1>^ arr = System::Text::Encoding::UTF8->GetBytes(chArray);
    char* c_arr = new char[arr->Length + 1];
    memset(c_arr, NULL, arr->Length + 1);
    System::IntPtr c_arr_ptr(c_arr);
    System::Runtime::InteropServices::Marshal::Copy(arr, 0, c_arr_ptr, arr->Length);
    cv::Mat img = imread(std::string(c_arr) + "\\Assets\\SplashScreen.scale-200.png", cv::IMREAD_COLOR);
    namedWindow("Display window", cv::WINDOW_AUTOSIZE);
    imshow("Display window", img);
    cv::waitKey(0);
}

String^ Img::getProjectPath()
{
    const char* projectPath = nullptr;
    projectPath = _getcwd(nullptr, 1);
    // std::string filePath(projectPath);
    String^ str = gcnew String(projectPath);
    return str;
}
