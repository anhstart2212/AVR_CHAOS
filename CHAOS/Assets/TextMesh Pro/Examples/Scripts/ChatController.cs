using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x020000AD RID: 173
public class ChatController : MonoBehaviour
{
    // Token: 0x060005AA RID: 1450 RVA: 0x00030652 File Offset: 0x0002E852
    private void OnEnable()
    {
        this.TMP_ChatInput.onSubmit.AddListener(new UnityAction<string>(this.AddToChatOutput));
    }

    // Token: 0x060005AB RID: 1451 RVA: 0x00030670 File Offset: 0x0002E870
    private void OnDisable()
    {
        this.TMP_ChatInput.onSubmit.RemoveListener(new UnityAction<string>(this.AddToChatOutput));
    }

    // Token: 0x060005AC RID: 1452 RVA: 0x00030690 File Offset: 0x0002E890
    private void AddToChatOutput(string newText)
    {
        this.TMP_ChatInput.text = string.Empty;
        DateTime now = DateTime.Now;
        TMP_Text tmp_ChatOutput = this.TMP_ChatOutput;
        string text = tmp_ChatOutput.text;
        tmp_ChatOutput.text = string.Concat(new string[]
        {
            text,
            "[<#FFFF80>",
            now.Hour.ToString("d2"),
            ":",
            now.Minute.ToString("d2"),
            ":",
            now.Second.ToString("d2"),
            "</color>] ",
            newText,
            "\n"
        });
        this.TMP_ChatInput.ActivateInputField();
        this.ChatScrollbar.value = 0f;
    }

    // Token: 0x04000657 RID: 1623
    public TMP_InputField TMP_ChatInput;

    // Token: 0x04000658 RID: 1624
    public TMP_Text TMP_ChatOutput;

    // Token: 0x04000659 RID: 1625
    public Scrollbar ChatScrollbar;
}
