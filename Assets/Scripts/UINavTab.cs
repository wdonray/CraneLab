using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UINavTab : MonoBehaviour {

    EventSystem system;

    void Start()
    {
        system = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            Selectable next = shift ?
                system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp() :
                system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));  
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            Button button = system.currentSelectedGameObject.GetComponent<Button>();
            button = button == null ? system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().GetComponent<Button>() : button;
            if (button == null) return;
            else button.onClick.Invoke();
        }

    }
}
