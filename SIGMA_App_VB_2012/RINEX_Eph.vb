'''''SIGMA'''''
'By: Ryan Brazeal
'Date: April 2015
'GNU GPL V2 License
'www.rgbi.ca
'www.jrpi.ca

Public Class RINEX_Eph
    Private PRN_Value As Integer
    Private Toc_Year_Value As Integer
    Private Toc_Month_Value As Integer
    Private Toc_Day_Value As Integer
    Private Toc_Hour_Value As Integer
    Private Toc_Minute_Value As Integer
    Private Toc_Second_Value As Decimal
    Private Clock_Bias_Value As Decimal
    Private Clock_Drift_Value As Decimal
    Private Clock_DriftRate_Value As Decimal
    Private IODE_Value As Decimal
    Private Crs_Value As Decimal
    Private Delta_n_Value As Decimal
    Private M0_Value As Decimal
    Private Cuc_Value As Decimal
    Private e_Value As Decimal
    Private Cus_Value As Decimal
    Private a_Value As Decimal
    Private Toe_Value As Decimal
    Private Cic_Value As Decimal
    Private Big_Omega_Value As Decimal
    Private Cis_Value As Decimal
    Private i0_Value As Decimal
    Private Crc_Value As Decimal
    Private Little_Omega_Value As Decimal
    Private Omega_DOT_Value As Decimal
    Private i_DOT_Value As Decimal
    Private L2_Codes_Value As Decimal
    Private Toe_Week_Value As Decimal
    Private L2_P_Flag_Value As Decimal
    Private SV_accuracy_Value As Decimal
    Private SV_health_Value As Decimal
    Private TGD_Value As Decimal
    Private IODC_Value As Decimal
    Private Transmission_of_Message_Value As Decimal
    Private Fit_Interval_Value As Decimal
    Private Spare1_Value As Decimal
    Private Spare2_Value As Decimal

    Public Sub New()
        PRN_Value = -9999I
        Toc_Year_Value = -9999I
        Toc_Month_Value = -9999I
        Toc_Day_Value = -9999I
        Toc_Hour_Value = -9999I
        Toc_Minute_Value = -9999I
        Toc_Second_Value = -9999D
        Clock_Bias_Value = -9999D
        Clock_Drift_Value = -9999D
        Clock_DriftRate_Value = -9999D
        IODE_Value = -9999D
        Crs_Value = -9999D
        Delta_n_Value = -9999D
        M0_Value = -9999D
        Cuc_Value = -9999D
        e_Value = -9999D
        Cus_Value = -9999D
        a_Value = -9999D
        Toe_Value = -9999D
        Cic_Value = -9999D
        Big_Omega_Value = -9999D
        Cis_Value = -9999D
        i0_Value = -9999D
        Crc_Value = -9999D
        Little_Omega_Value = -9999D
        Omega_DOT_Value = -9999D
        i_DOT_Value = -9999D
        L2_Codes_Value = -9999D
        Toe_Week_Value = -9999D
        L2_P_Flag_Value = -9999D
        SV_accuracy_Value = -9999D
        SV_health_Value = -9999D
        TGD_Value = -9999D
        IODC_Value = -9999D
        Transmission_of_Message_Value = -9999D
        Fit_Interval_Value = -9999D
        Spare1_Value = -9999D
        Spare2_Value = -9999D
    End Sub

    Public Property PRN() As Integer
        Get
            Return PRN_Value
        End Get

        Set(ByVal value As Integer)
            PRN_Value = value
        End Set
    End Property

    Public Property Toc_Year() As Integer
        Get
            Return Toc_Year_Value
        End Get

        Set(ByVal value As Integer)
            Toc_Year_Value = value
        End Set
    End Property

    Public Property Toc_Month() As Integer
        Get
            Return Toc_Month_Value
        End Get

        Set(ByVal value As Integer)
            Toc_Month_Value = value
        End Set
    End Property

    Public Property Toc_Day() As Integer
        Get
            Return Toc_Day_Value
        End Get

        Set(ByVal value As Integer)
            Toc_Day_Value = value
        End Set
    End Property

    Public Property Toc_Hour() As Integer
        Get
            Return Toc_Hour_Value
        End Get

        Set(ByVal value As Integer)
            Toc_Hour_Value = value
        End Set
    End Property

    Public Property Toc_Minute() As Integer
        Get
            Return Toc_Minute_Value
        End Get

        Set(ByVal value As Integer)
            Toc_Minute_Value = value
        End Set
    End Property

    Public Property Toc_Second() As Decimal
        Get
            Return Toc_Second_Value
        End Get

        Set(ByVal value As Decimal)
            Toc_Second_Value = value
        End Set
    End Property

    Public Property Clock_Bias() As Decimal
        Get
            Return Clock_Bias_Value
        End Get

        Set(ByVal value As Decimal)
            Clock_Bias_Value = value
        End Set
    End Property

    Public Property Clock_Drift() As Decimal
        Get
            Return Clock_Drift_Value
        End Get

        Set(ByVal value As Decimal)
            Clock_Drift_Value = value
        End Set
    End Property

    Public Property Clock_DriftRate() As Decimal
        Get
            Return Clock_DriftRate_Value
        End Get

        Set(ByVal value As Decimal)
            Clock_DriftRate_Value = value
        End Set
    End Property

    Public Property IODE() As Decimal
        Get
            Return IODE_Value
        End Get

        Set(ByVal value As Decimal)
            IODE_Value = value
        End Set
    End Property

    Public Property Crs() As Decimal
        Get
            Return Crs_Value
        End Get

        Set(ByVal value As Decimal)
            Crs_Value = value
        End Set
    End Property

    Public Property Delta_n() As Decimal
        Get
            Return Delta_n_Value
        End Get

        Set(ByVal value As Decimal)
            Delta_n_Value = value
        End Set
    End Property

    Public Property M0() As Decimal
        Get
            Return M0_Value
        End Get

        Set(ByVal value As Decimal)
            M0_Value = value
        End Set
    End Property

    Public Property Cuc() As Decimal
        Get
            Return Cuc_Value
        End Get

        Set(ByVal value As Decimal)
            Cuc_Value = value
        End Set
    End Property

    Public Property e() As Decimal
        Get
            Return e_Value
        End Get

        Set(ByVal value As Decimal)
            e_Value = value
        End Set
    End Property

    Public Property Cus() As Decimal
        Get
            Return Cus_Value
        End Get

        Set(ByVal value As Decimal)
            Cus_Value = value
        End Set
    End Property

    Public Property a() As Decimal
        Get
            Return a_Value
        End Get

        Set(ByVal value As Decimal)
            a_Value = value
        End Set
    End Property

    Public Property Toe() As Decimal
        Get
            Return Toe_Value
        End Get

        Set(ByVal value As Decimal)
            Toe_Value = value
        End Set
    End Property

    Public Property Cic() As Decimal
        Get
            Return Cic_Value
        End Get

        Set(ByVal value As Decimal)
            Cic_Value = value
        End Set
    End Property

    Public Property Big_Omega() As Decimal
        Get
            Return Big_Omega_Value
        End Get

        Set(ByVal value As Decimal)
            Big_Omega_Value = value
        End Set
    End Property

    Public Property Cis() As Decimal
        Get
            Return Cis_Value
        End Get

        Set(ByVal value As Decimal)
            Cis_Value = value
        End Set
    End Property

    Public Property i0() As Decimal
        Get
            Return i0_Value
        End Get

        Set(ByVal value As Decimal)
            i0_Value = value
        End Set
    End Property

    Public Property Crc() As Decimal
        Get
            Return Crc_Value
        End Get

        Set(ByVal value As Decimal)
            Crc_Value = value
        End Set
    End Property

    Public Property Little_Omega() As Decimal
        Get
            Return Little_Omega_Value
        End Get

        Set(ByVal value As Decimal)
            Little_Omega_Value = value
        End Set
    End Property

    Public Property Omega_DOT() As Decimal
        Get
            Return Omega_DOT_Value
        End Get

        Set(ByVal value As Decimal)
            Omega_DOT_Value = value
        End Set
    End Property

    Public Property i_DOT() As Decimal
        Get
            Return i_DOT_Value
        End Get

        Set(ByVal value As Decimal)
            i_DOT_Value = value
        End Set
    End Property

    Public Property L2_Codes() As Decimal
        Get
            Return L2_Codes_Value
        End Get

        Set(ByVal value As Decimal)
            L2_Codes_Value = value
        End Set
    End Property

    Public Property Toe_Week() As Decimal
        Get
            Return Toe_Week_Value
        End Get

        Set(ByVal value As Decimal)
            Toe_Week_Value = value
        End Set
    End Property

    Public Property L2_P_Flag() As Decimal
        Get
            Return L2_P_Flag_Value
        End Get

        Set(ByVal value As Decimal)
            L2_P_Flag_Value = value
        End Set
    End Property

    Public Property SV_accuracy() As Decimal
        Get
            Return SV_accuracy_Value
        End Get

        Set(ByVal value As Decimal)
            SV_accuracy_Value = value
        End Set
    End Property

    Public Property SV_health() As Decimal
        Get
            Return SV_health_Value
        End Get

        Set(ByVal value As Decimal)
            SV_health_Value = value
        End Set
    End Property

    Public Property TGD() As Decimal
        Get
            Return TGD_Value
        End Get

        Set(ByVal value As Decimal)
            TGD_Value = value
        End Set
    End Property

    Public Property IODC() As Decimal
        Get
            Return IODC_Value
        End Get

        Set(ByVal value As Decimal)
            IODC_Value = value
        End Set
    End Property

    Public Property Transmission_of_Message() As Decimal
        Get
            Return Transmission_of_Message_Value
        End Get

        Set(ByVal value As Decimal)
            Transmission_of_Message_Value = value
        End Set
    End Property

    Public Property Fit_Interval() As Decimal
        Get
            Return Fit_Interval_Value
        End Get

        Set(ByVal value As Decimal)
            Fit_Interval_Value = value
        End Set
    End Property

    Public Property Spare1() As Decimal
        Get
            Return Spare1_Value
        End Get

        Set(ByVal value As Decimal)
            Spare1_Value = value
        End Set
    End Property

    Public Property Spare2() As Decimal
        Get
            Return Spare2_Value
        End Get

        Set(ByVal value As Decimal)
            Spare2_Value = value
        End Set
    End Property

End Class
