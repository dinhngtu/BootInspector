namespace TcgLog {
    [EnumFormat(EnumFormatType.Hex)]
    [Source("edk2/Tcg2Protocol.h")]
    internal enum TCG_EVENTTYPE : UInt32 {
        //
        // Standard event types
        //
        EV_PREBOOT_CERT = 0x00000000,
        EV_POST_CODE = 0x00000001,
        EV_NO_ACTION = 0x00000003,
        EV_SEPARATOR = 0x00000004,
        EV_ACTION = 0x00000005,
        EV_EVENT_TAG = 0x00000006,
        EV_S_CRTM_CONTENTS = 0x00000007,
        EV_S_CRTM_VERSION = 0x00000008,
        EV_CPU_MICROCODE = 0x00000009,
        EV_PLATFORM_CONFIG_FLAGS = 0x0000000A,
        EV_TABLE_OF_DEVICES = 0x0000000B,
        EV_COMPACT_HASH = 0x0000000C,
        EV_NONHOST_CODE = 0x0000000F,
        EV_NONHOST_CONFIG = 0x00000010,
        EV_NONHOST_INFO = 0x00000011,
        EV_OMIT_BOOT_DEVICE_EVENTS = 0x00000012,

        //
        // EFI specific event types
        //
        EV_EFI_EVENT_BASE = 0x80000000,
        EV_EFI_VARIABLE_DRIVER_CONFIG = EV_EFI_EVENT_BASE + 1,
        EV_EFI_VARIABLE_BOOT = EV_EFI_EVENT_BASE + 2,
        EV_EFI_BOOT_SERVICES_APPLICATION = EV_EFI_EVENT_BASE + 3,
        EV_EFI_BOOT_SERVICES_DRIVER = EV_EFI_EVENT_BASE + 4,
        EV_EFI_RUNTIME_SERVICES_DRIVER = EV_EFI_EVENT_BASE + 5,
        EV_EFI_GPT_EVENT = EV_EFI_EVENT_BASE + 6,
        EV_EFI_ACTION = EV_EFI_EVENT_BASE + 7,
        EV_EFI_PLATFORM_FIRMWARE_BLOB = EV_EFI_EVENT_BASE + 8,
        EV_EFI_HANDOFF_TABLES = EV_EFI_EVENT_BASE + 9,
        EV_EFI_PLATFORM_FIRMWARE_BLOB2 = EV_EFI_EVENT_BASE + 0xA,
        EV_EFI_HANDOFF_TABLES2 = EV_EFI_EVENT_BASE + 0xB,
        EV_EFI_HCRTM_EVENT = EV_EFI_EVENT_BASE + 0x10,
        EV_EFI_VARIABLE_AUTHORITY = EV_EFI_EVENT_BASE + 0xE0,
        EV_EFI_SPDM_FIRMWARE_BLOB = EV_EFI_EVENT_BASE + 0xE1,
        EV_EFI_SPDM_FIRMWARE_CONFIG = EV_EFI_EVENT_BASE + 0xE2,
        //#define EV_EFI_SPDM_DEVICE_BLOB           EV_EFI_SPDM_FIRMWARE_BLOB
        //#define EV_EFI_SPDM_DEVICE_CONFIG         EV_EFI_SPDM_FIRMWARE_CONFIG

        /// <summary>
        /// The SPDM policy database for SPDM verification.
        /// It goes to PCR7
        /// </summary>
        EV_EFI_SPDM_DEVICE_POLICY = EV_EFI_EVENT_BASE + 0xE3,

        /// <summary>
        /// The SPDM policy authority for SPDM verification for the signature
        /// of GET_MEASUREMENT or CHALLENGE_AUTH. It goes to PCR7.
        /// </summary>
        EV_EFI_SPDM_DEVICE_AUTHORITY = EV_EFI_EVENT_BASE + 0xE4,
    }

    [EnumFormat(EnumFormatType.Hex)]
    [Source("edk2/Tpm20.h")]
    internal enum TPM_ALG_ID : UInt16 {
        TPM_ALG_ERROR = 0x0000,
        TPM_ALG_FIRST = 0x0001,
        // #define TPM_ALG_RSA            (TPM_ALG_ID)(0x0001)
        // #define TPM_ALG_SHA            (TPM_ALG_ID)(0x0004)
        TPM_ALG_SHA1 = 0x0004,
        // #define TPM_ALG_HMAC           (TPM_ALG_ID)(0x0005)
        TPM_ALG_AES = 0x0006,
        // #define TPM_ALG_MGF1           (TPM_ALG_ID)(0x0007)
        TPM_ALG_KEYEDHASH = 0x0008,
        // #define TPM_ALG_XOR            (TPM_ALG_ID)(0x000A)
        TPM_ALG_SHA256 = 0x000B,
        TPM_ALG_SHA384 = 0x000C,
        TPM_ALG_SHA512 = 0x000D,
        TPM_ALG_NULL = 0x0010,
        TPM_ALG_SM3_256 = 0x0012,
        TPM_ALG_SM4 = 0x0013,
        TPM_ALG_RSASSA = 0x0014,
        TPM_ALG_RSAES = 0x0015,
        TPM_ALG_RSAPSS = 0x0016,
        TPM_ALG_OAEP = 0x0017,
        TPM_ALG_ECDSA = 0x0018,
        TPM_ALG_ECDH = 0x0019,
        TPM_ALG_ECDAA = 0x001A,
        TPM_ALG_SM2 = 0x001B,
        TPM_ALG_ECSCHNORR = 0x001C,
        TPM_ALG_ECMQV = 0x001D,
        TPM_ALG_KDF1_SP800_56a = 0x0020,
        TPM_ALG_KDF2 = 0x0021,
        TPM_ALG_KDF1_SP800_108 = 0x0022,
        TPM_ALG_ECC = 0x0023,
        TPM_ALG_SYMCIPHER = 0x0025,
        TPM_ALG_CTR = 0x0040,
        TPM_ALG_OFB = 0x0041,
        TPM_ALG_CBC = 0x0042,
        TPM_ALG_CFB = 0x0043,
        TPM_ALG_ECB = 0x0044,
        TPM_ALG_LAST = 0x0044,
    }
}
