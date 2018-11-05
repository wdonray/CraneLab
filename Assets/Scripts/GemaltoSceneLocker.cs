using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Aladdin.HASP;

public class GemaltoSceneLocker : MonoBehaviour
{

    private void Start()
    {
        HaspFeature feature = HaspFeature.Default;

        string scope =
        "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
        "<haspscope/>";

        string vendorCode =
        "pEl9GkEx50FvJgYEJRAQZRitr7/xkSIWx8dqPJ6lwu9qZIY4AZJvFhOqDN9DC0bgfq+GUcbR/1URnMaL" +
        "660OVvvBntLtBV9mfhRs91g5AWQO/zzCvQG/RwpCVElxZy2X7u1GQWR7iqGEmG47GzGtFCZmnwAv2fLt" +
        "0zXaCLZNGSv84Sk1pYdHqv2hH2eSpLOL6F7OCYSq2viRRRNY0moXZ2OCs8sbmKcsqKoobrenH/QYOYMF" +
        "oVaJ6oCcCoDhpdZd6E3nUmpV4mf3lw8DoWZkS09MyL8YmlRWPURuN0BFsL/VltYnA6kZHOMP8afmCGWm" +
        "Cg6wL0I2OdoxCWW26+YxazXCe+VO5Uex6geN0gj6/RAEOVDUV+vY0yimetOu/kLk6OKv3UA0zOHKI/aB" +
        "TgRAceHycLEyEbB070ufH5/rb9yHVKEK7aFfFxGv4m0FvavFWb1knsmG8pEWxm17nblwoGjdBiHm0CRf" +
        "8UI+4znSOBORLdUcKN8O6q8JGLJMjRzOr8g7Xk1isQGJtCPMVxFSW+Ukmmcjc1w/+j45yzlZvWfWffPo" +
        "wAzpZ17PKGhMJaj95ECm7GpVFSQ2iuyx1NoJ4cCWgpf9v4zKEr3jig1+uuX1pODNhjo9JoiQ/PN1alMM" +
        "MpPFgn/HMTK8iiq35gxaum6xX1qKVLsm4YTdEG4zKQOnWUNVdsi9beVLOstmCr2bHI3wrD47g++jZlVK" +
        "el4+K1yL1ObGRFvbvuKUWUMRfKBsigt758IsXwsoIfY0FiUADeXoI6gQ3XP4hqseQBSpmvaQA0JjThqc" +
        "eGMdnL8WbWrMhcmCqmLNMoyVTNF7YwSNkAILE3QPi8iI8SpJzhpQ7LnrOVJ7iWkFmTbq5HqCHnqU2wFv" +
        "jk+nAxJFBA3RBrVztQGfO5Y+ZwVxb2XxgJFsgpm5Vd/5big7owqNIdRTY25sZave6oqqm9475QeRsSvX" +
        "3sY3KQL+YFo8nTMjc5UENg==";

        Hasp hasp = new Hasp(feature);
        HaspStatus status = hasp.Login(vendorCode, scope);

        if (HaspStatus.StatusOk != status)
        {
            Debug.Log("Security Key Not Found \n" +
                status.ToString());
        }
        else if (HaspStatus.StatusOk == status)
        {
            Debug.Log("All good");
        }
    }
}
