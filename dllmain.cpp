#include "pch.h"
#include <Windows.h>
#include <string>
extern "C"
{
	__declspec(dllexport) int inject(const DWORD procId, const char* dllPath)
	{

		if (procId == 0 || dllPath == 0) {
			//std::wcout << "Could not find " << processName.c_str() << std::endl;
			return 0;
		}
		else {
			//std::wcout << "Process ID is " << processID << std::endl;

			// Open a handle to target process
			HANDLE hProcess = OpenProcess(PROCESS_ALL_ACCESS, FALSE, procId);

			if (hProcess == 0) {
				return 0;
			}
			// Allocate memory for the dllpath in the target process
			// length of the path string + null terminator
			LPVOID pDllPath = VirtualAllocEx(hProcess, 0, strlen((LPCSTR)dllPath) + 1,
				MEM_COMMIT, PAGE_READWRITE);

			// Write the path to the address of the memory we just allocated
			// in the target process
			if (WriteProcessMemory(hProcess, pDllPath, (LPVOID)dllPath,
				strlen((LPCSTR)dllPath) + 1, 0) == false) {
				return 0;
			}

			// Create a Remote Thread in the target process which
			// calls LoadLibraryA as our dllpath as an argument -> program loads our dll
			HANDLE hLoadThread = CreateRemoteThread(hProcess, 0, 0,
				(LPTHREAD_START_ROUTINE)GetProcAddress(GetModuleHandleA("Kernel32.dll"),
					"LoadLibraryA"), pDllPath, 0, 0);

			if (hLoadThread == 0) {
				return 0;
			}
			// Wait for the execution of our loader thread to finish
			WaitForSingleObject(hLoadThread, INFINITE);

			//std::cout << "Dll path allocated at: " << std::hex << pDllPath << std::endl;
			//std::cin.get();

			// Free the memory allocated for our dll path
			VirtualFreeEx(hProcess, pDllPath, strlen((LPCSTR)dllPath) + 1, MEM_RELEASE);

			CloseHandle(hProcess);
			return 1;
		}
	}
	__declspec(dllexport) void ShowMe()
	{
		MessageBox(0, L"Im Working", L"Hi", MB_ICONINFORMATION);
	}
}
