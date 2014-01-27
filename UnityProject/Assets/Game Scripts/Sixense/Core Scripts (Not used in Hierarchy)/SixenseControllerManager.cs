using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class SixenseControllerManager {
	
	[DllImport ("sixense")]
	private static extern int sixenseInit();
	
	public static void InitializeSixense()
	{
		var result = sixenseInit();
		if(result == (int)SixenseStatus.SIXENSE_FAILURE)
		{
			Debug.Log("Sixense Initialization Failed. See SixenseControllerManager.InitializeSixense()");
		}
	}
	
	[DllImport ("sixense")]
	private static extern int sixenseExit();
	
	public static void ExitSixense()
	{
		var result = sixenseExit();
		if(result == (int)SixenseStatus.SIXENSE_FAILURE)
		{
			Debug.Log("Sixense Exit Failed. See SixenseControllerManager.ExitSixense()");
		}
	}
	
	[DllImport ("sixense")]
	private static extern int sixenseGetMaxBases();
	
	[DllImport ("sixense")]
	private static extern int sixenseSetActiveBase(int i);
	
	[DllImport ("sixense")]
	private static extern int sixenseIsBaseConnected(int i);
	
	[DllImport ("sixense")]
	private static extern int sixenseGetMaxControllers();
	
	[DllImport ("sixense")]
	private static extern int sixenseIsControllerEnabled(int which);
	
	public static bool IsControllerEnabled(SixenseControllerHand hand)
	{
		return sixenseIsControllerEnabled((int)hand) == 1 ? true : false;
	}
	
	[DllImport ("sixense")]
	private static extern int sixenseGetNumActiveControllers();
	
	public static int GetNumberOfActiveControllers()
	{
		return sixenseGetNumActiveControllers();
	}
	
	[DllImport ("sixense")]
	private static extern int sixenseGetHistorySize();
	
	[DllImport ("sixense")]
	private static extern int sixenseGetData(int which, int index_back, out sixenseControllerRawData data);
	
	[DllImport ("sixense")]
	private static extern int sixenseGetAllData(int index_back, out sixenseAllControllerRawData data);
	
	[DllImport ("sixense")]
	private static extern int sixenseGetNewestData(int which, out sixenseControllerRawData data);
	
	public static SixenseControllerData GetControllerData(SixenseControllerHand hand)
	{
		sixenseControllerRawData rawData;
		sixenseGetNewestData((int)hand, out rawData);
		return new SixenseControllerData(rawData);
	}
		                                      
	[DllImport ("sixense")]
	private static extern int sixenseGetAllNewestData(out sixenseAllControllerRawData data);
	
	public static List<SixenseControllerData> GetAllControllerData()
	{
		List<SixenseControllerData> data = new List<SixenseControllerData>();
		sixenseAllControllerRawData rawData;
		sixenseGetAllNewestData(out rawData);
		foreach(var controller in rawData.controllers)
		{
			data.Add(new SixenseControllerData(controller));
		}
		return data;
	}
	
	[DllImport ("sixense")]
	private static extern int sixenseSetFilterEnabled(int on_or_off);
	
	[DllImport ("sixense")]
	private static extern int sixenseGetFilterEnabled(out int on_or_off);
	
	[DllImport ("sixense")]
	private static extern int sixenseSetFilterParams(float near_range, float near_val,
	                                                 float far_range, float far_val);
	
	public static SixenseStatus SetFilterParameters(FilterParameters filters)
	{
		return sixenseSetFilterParams(filters.NearRange, filters.NearValue, filters.FarRange, filters.FarValue) == (int)SixenseStatus.SIXENSE_SUCCESS 
			? SixenseStatus.SIXENSE_SUCCESS : SixenseStatus.SIXENSE_FAILURE;
	}
	
	[DllImport ("sixense")]
	private static extern int sixenseGetFilterParams(out float near_range, out float near_val,
	                                                 out float far_range, out float far_val);
	
	public static FilterParameters GetFilterParameters()
	{
		float nearRange, nearValue, farRange, farValue;
		sixenseGetFilterParams(out nearRange, out nearValue, out farRange, out farValue);
		return new FilterParameters(nearRange, nearValue, farRange, farValue);
	}
	
}
