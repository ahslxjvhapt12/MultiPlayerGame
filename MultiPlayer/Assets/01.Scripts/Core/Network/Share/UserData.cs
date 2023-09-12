using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class UserData
{
    public string username;
    public string userAuthId;

    public ArraySegment<byte> Serialize()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(1024);

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        ushort nameLen = (ushort)Encoding.UTF8.GetByteCount(username);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
        count += sizeof(ushort);

        byte[] namearr = Encoding.UTF8.GetBytes(username);
        Array.Copy(namearr, 0, segment.Array, count, nameLen);
        count += nameLen;

        ushort authLen = (ushort)Encoding.UTF8.GetByteCount(userAuthId);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), authLen);
        count += sizeof(ushort);

        byte[] authArr = Encoding.UTF8.GetBytes(userAuthId);
        Array.Copy(authArr, 0, segment.Array, count, nameLen);
        count += nameLen;

        // username을 UTF8 방식으로 인코딩했을때 바이트 배열을 만들어 준다.
        //byte[] strBuffer = Encoding.UTF8.GetBytes(username);
        //ushort strlen = (ushort)strBuffer.Length;
        //byte[] lenBuffer = BitConverter.GetBytes(strlen); // 2바이트 짜리 바이트 배열을 만들어서 저장한다.
        //byte[] result = new byte[lenBuffer.Length + strBuffer.Length];
        //Array.Copy(lenBuffer, 0, result, 0, lenBuffer.Length);

        if (!success)
        {
            Debug.LogError("Packet serialize error!");
            return null;
        }

        return SendBufferHelper.Close(count);
    }

    public void Deserialize(byte[] payload)
    {
        int count = 0;
        ushort nameLen = BitConverter.ToUInt16(payload, count); // 앞에 2바이트만 잘라서 변환
        count += sizeof(ushort);
        username = Encoding.UTF8.GetString(payload, count, nameLen);
        count += nameLen;

        ushort authLen = BitConverter.ToUInt16(payload, count);
        count += sizeof(ushort);
        userAuthId = Encoding.UTF8.GetString(payload, count, authLen);
    }
}