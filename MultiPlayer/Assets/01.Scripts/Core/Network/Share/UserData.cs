using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UserData
{
    public string username;
    public byte[] Serialize()
    {
        // username을 UTF8 방식으로 인코딩했을때 바이트 배열을 만들어 준다.
        byte[] strBuffer = Encoding.UTF8.GetBytes(username);
        ushort strlen = (ushort)strBuffer.Length;
        byte[] lenBuffer = BitConverter.GetBytes(strlen); // 2바이트 짜리 바이트 배열을 만들어서 저장한다.

        byte[] result = new byte[lenBuffer.Length + strBuffer.Length];
        Array.Copy(lenBuffer, 0, result, 0, lenBuffer.Length);
        Array.Copy(strBuffer, 0, result, lenBuffer.Length, strBuffer.Length);

        return result;
    }

    public void Deserialize(byte[] payload)
    {
        ushort len = BitConverter.ToUInt16(payload, 0); // 앞에 2바이트만 잘라서 변환
        username = Encoding.UTF8.GetString(payload, 2, len);
    }
}
