#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN
// Windows Header Files:
#include <windows.h>

// DWORD key, WORD repeats, BYTE scanCode, BOOL isExtended, BOOL isWithAlt, BOOL wasDownBefore, BOOL isUpNow
typedef void(*KeyboardHandler)(DWORD, WORD, BYTE, BOOL, BOOL, BOOL, BOOL);