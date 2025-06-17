using System;

[Serializable]
public class SmartContractSuccessResponse
{
    public string status;
    public string[] data;
}

[Serializable]
public class SmartContractErrorDetail
{
    public int code;
    public string errorCode;
    public string message;
    public string trace_id;
    public string details;
}

[Serializable]
public class SmartContractErrorResponse
{
    public string status;
    public SmartContractErrorDetail error;
}
