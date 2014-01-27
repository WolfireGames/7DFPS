using System;
using System.Collections;
using UnityEngine;

public struct sixenseControllerRawData
{
	public Vector3 Position;
	public Vector3 RotationX;
	public Vector3 RotationY;
	public Vector3 RotationZ;
	public float JoystickX;
	public float JoystickY;
	public float Trigger;
	public int Buttons;
	public byte SequenceNumber;
	public Quaternion RotationQuaternion;
	public short FirmwareRevision;
	public short HardwareRevision;
	public short PacketType;
	public short MagneticFrequency;
	public int Enabled;
	public int ControllerIndex;
	public byte IsDocked;
	public byte WhichHand;
}

public struct sixenseAllControllerRawData
{
	public sixenseControllerRawData[] controllers;
}
