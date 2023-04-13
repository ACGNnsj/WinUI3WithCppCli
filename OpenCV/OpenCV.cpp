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
    msclr::interop::marshal_context context;
    /*IntPtr p = Runtime::InteropServices::Marshal::StringToHGlobalAnsi(path);
    const char* str = static_cast<char*>(p.ToPointer());
    Runtime::InteropServices::Marshal::FreeHGlobal(p);*/
    auto str = context.marshal_as<std::string>(path);
    cv::Mat img = imread(str, cv::IMREAD_COLOR);
    namedWindow("Display window", cv::WINDOW_AUTOSIZE);
    imshow("Display window", img);
    cv::waitKey(0);
}

void Img::showImgAlter(String^ path)
{
    /*cli::array<wchar_t>^ chArray = path->ToCharArray();
    cli::array<unsigned char, 1>^ arr = System::Text::Encoding::UTF8->GetBytes(chArray);
    char* c_arr = new char[arr->Length + 1];
    memset(c_arr, NULL, arr->Length + 1);
    System::IntPtr c_arr_ptr(c_arr);
    System::Runtime::InteropServices::Marshal::Copy(arr, 0, c_arr_ptr, arr->Length);*/
    // msclr::interop::marshal_context context;
    // std::string str= context.marshal_as<std::string>(path);
    IntPtr p = Runtime::InteropServices::Marshal::StringToHGlobalAnsi(path);
    const char* str = static_cast<char*>(p.ToPointer());
    // system(str);
    Runtime::InteropServices::Marshal::FreeHGlobal(p);

    FILE* f;
    errno_t err = fopen_s(&f, str, "rb");
    if (!err)
    {
        fseek(f, 0, SEEK_END);
        const size_t buffer_size = ftell(f);
        fseek(f, 0, SEEK_SET);
        std::vector<char> buffer(buffer_size);
        fread(&buffer[0], sizeof(char), buffer_size, f);
        fclose(f);
        // cv::Mat img = imread(std::string(c_arr), cv::IMREAD_COLOR);
        cv::Mat img = imdecode(buffer, cv::IMREAD_COLOR);
        namedWindow("Display window", cv::WINDOW_AUTOSIZE);
        imshow("Display window", img);
        cv::waitKey(0);
    }
    else
    {
        namedWindow(str, cv::WINDOW_AUTOSIZE);
    }
}


void Img::showImgDefault(String^ path)
{
    showImg(path + "\\Assets\\SplashScreen.scale-200.png");
}

String^ Img::getProjectPath()
{
    const char* projectPath = nullptr;
    projectPath = _getcwd(nullptr, 1);
    // std::string filePath(projectPath);
    String^ str = gcnew String(projectPath);
    return str;
}
