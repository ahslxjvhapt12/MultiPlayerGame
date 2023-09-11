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
        // username�� UTF8 ������� ���ڵ������� ����Ʈ �迭�� ����� �ش�.
        byte[] strBuffer = Encoding.UTF8.GetBytes(username);
        ushort strlen = (ushort)strBuffer.Length;
        byte[] lenBuffer = BitConverter.GetBytes(strlen); // 2����Ʈ ¥�� ����Ʈ �迭�� ���� �����Ѵ�.

        byte[] result = new byte[lenBuffer.Length + strBuffer.Length];
        Array.Copy(lenBuffer, 0, result, 0, lenBuffer.Length);
        Array.Copy(strBuffer, 0, result, lenBuffer.Length, strBuffer.Length);

        return result;
    }

    public void Deserialize(byte[] payload)
    {
        ushort len = BitConverter.ToUInt16(payload, 0); // �տ� 2����Ʈ�� �߶� ��ȯ
        username = Encoding.UTF8.GetString(payload, 2, len);
    }
}
