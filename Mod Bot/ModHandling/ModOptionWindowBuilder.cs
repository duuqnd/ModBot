﻿using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using InternalModBot;

#pragma warning disable IDE1005 // Delegate invocation can be simplefied

namespace ModLibrary
{
    /// <summary>
    /// Used to place all of the options in the options window
    /// </summary>
    public class ModOptionsWindowBuilder
    {
        readonly GameObject _content;
        readonly GameObject _spawnedBase;
        readonly Button _xButton;
        readonly GameObject _owner;
        readonly Mod _ownerMod;

        internal ModOptionsWindowBuilder(GameObject owner, Mod ownerMod)
        {
            GameUIRoot.Instance.SetEscMenuDisabled(true);
            RegisterShouldCursorBeEnabledDelegate.Register(shouldCurorBeEnabled);
            GameUIRoot.Instance.RefreshCursorEnabled();

            owner.SetActive(false);
            _owner = owner;
            _ownerMod = ownerMod;
            GameObject modsWindowPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModOptionsCanvas", "Clone Drone in the Danger Zone_Data/");
            _spawnedBase = GameObject.Instantiate(modsWindowPrefab);
            _spawnedBase.AddComponent<CloseModOptionsWindowOnEscapeKey>().Init(this); // used to make sure we can close the window with escape
            ModdedObject modObject = _spawnedBase.GetComponent<ModdedObject>();
            _content = modObject.GetObject<GameObject>(0);
            _xButton = modObject.GetObject<Button>(1);
            _xButton.onClick.AddListener(CloseWindow);
        }

        /// <summary>
        /// Adds a slider, note that the value of the slider will be saved by Mod-Bot so you dont need to save it in a ny way
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">The name of the slider, this will both be displayed to the user and used in the mod to get the value (no 2 names should EVER be the same)</param>
        /// <param name="slider">A reference that is set to the created slider</param>
        /// <param name="onChange">A callback that gets called when the slider gets changed, if null wont do anything</param>
        public void AddSlider(float min, float max, float defaultValue, string name, out Slider slider, Action<float> onChange = null)
        {
            GameObject SliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(SliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = _content.transform;
            moddedObject.GetObject<Text>(0).text = name;
            slider = moddedObject.GetObject<Slider>(1);
            slider.minValue = min;
            slider.maxValue = max;
            slider.value = defaultValue;
            Text numberDisplay = moddedObject.GetObject<Text>(2);

            float? loadedFloat = OptionsSaver.LoadFloat(_ownerMod, name);
            if(loadedFloat.HasValue)
            {
                slider.value = loadedFloat.Value;
            }

            if(onChange != null)
            {
                onChange(slider.value);
            }

            numberDisplay.text = slider.value.ToString();

            slider.onValueChanged.AddListener(delegate (float value)
            {
                OptionsSaver.SaveFloat(_ownerMod, name, value);

                if(onChange != null)
                {
                    onChange(value);
                }

                numberDisplay.text = value.ToString();
            });
        }

        /// <summary>
        /// Adds a slider, note that the value of the slider will be saved by Mod-Bot so you dont need to save it in a ny way
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">The maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">The name of the slider, this will both be displayed to the user and used in the mod to get the value (no 2 names should EVER be the same)</param>
        /// <param name="onChange">A callback that gets called when the slider gets changed, if null wont do anything</param>
        public void AddSlider(float min, float max, float defaultValue, string name, Action<float> onChange = null)
        {
            AddSlider(min, max, defaultValue, name, out Slider slider, onChange);
        }

        /// <summary>
        /// Adds a slider to the options window that can only be whole numbers
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">That maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">Both the display name in the list and used by you to get the value (no 2 names should EVER be the same)</param>
        /// <param name="slider">A reference that is set to the created slider</param>
        /// <param name="onChange">Called when the value is changed, if null does nothing</param>
        public void AddIntSlider(int min, int max, int defaultValue, string name, out Slider slider, Action<int> onChange = null)
        {
            GameObject SliderPrefab = AssetLoader.GetObjectFromFile("modswindow", "Slider", "Clone Drone in the Danger Zone_Data/");
            ModdedObject moddedObject = GameObject.Instantiate(SliderPrefab).GetComponent<ModdedObject>();
            moddedObject.transform.parent = _content.transform;
            moddedObject.GetObject<Text>(0).text = name;
            Slider _slider = moddedObject.GetObject<Slider>(1);
            _slider.minValue = min;
            _slider.maxValue = max;
            _slider.wholeNumbers = true;
            _slider.value = defaultValue;
            Text numberDisplay = moddedObject.GetObject<Text>(2);

            int? loadedInt = OptionsSaver.LoadInt(_ownerMod, name);
            if(loadedInt.HasValue)
            {
                _slider.value = loadedInt.Value;
            }

            if(onChange != null)
            {
                onChange((int)_slider.value);
            }

            numberDisplay.text = _slider.value.ToString();
            _slider.onValueChanged.AddListener(delegate (float value)
            {
                OptionsSaver.SaveInt(_ownerMod, name, (int)value);

                if(onChange != null)
                {
                    onChange((int)_slider.value);
                }

                numberDisplay.text = value.ToString();
            });

            slider = _slider;
        }
        
        /// <summary>
        /// Adds a slider to the options window that can only be whole numbers
        /// </summary>
        /// <param name="min">The minimum value of the slider</param>
        /// <param name="max">That maximum value of the slider</param>
        /// <param name="defaultValue">The value the slider will be set to before it is changed by the user</param>
        /// <param name="name">Both the display name in the list and used by you to get the value (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Called when the value is changed, if null does nothing</param>
        public void AddIntSlider(int min, int max, int defaultValue, string name, Action<int> onChange = null)
        {
            AddIntSlider(min, max, defaultValue, name, out Slider slider, onChange);
        }
        /// <summary>
        /// Adds a checkbox to the mods window
        /// </summary>
        /// <param name="defaultValue">The value the checkbox will be set to before the user changes it</param>
        /// <param name="name">Both the display name of the checkbox and what you use to get the value of the checkbox (no 2 names should EVER be the same)</param>
        /// <param name="toggle">>A reference that is set to the created toggle</param>
        /// <param name="onChange">Called when the value of the checkbox is changed, if null does nothing</param>
        public void AddCheckbox(bool defaultValue, string name, out Toggle toggle, Action<bool> onChange = null)
        {
            GameObject CheckBoxPrefab = AssetLoader.GetObjectFromFile("modswindow", "Checkbox", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedObject = GameObject.Instantiate(CheckBoxPrefab);
            spawnedObject.transform.parent = _content.transform;
            ModdedObject moddedObject = spawnedObject.GetComponent<ModdedObject>();
            toggle = moddedObject.GetObject<Toggle>(0);
            toggle.isOn = defaultValue;
            moddedObject.GetObject<GameObject>(1).GetComponent<Text>().text = name;

            bool? loadedBool = OptionsSaver.LoadBool(_ownerMod, name);
            if (loadedBool.HasValue)
            {
                toggle.isOn = loadedBool.Value;
            }

            if (onChange != null)
            {
                onChange(toggle.isOn);
            }

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                OptionsSaver.SaveBool(_ownerMod, name, value);

                if (onChange != null)
                {
                    onChange(value);
                }
            });
        }
        /// <summary>
        /// Adds a checkbox to the mods window
        /// </summary>
        /// <param name="defaultValue">The value the checkbox will be set to before the user changes it</param>
        /// <param name="name">Both the display name of the checkbox and what you use to get the value of the checkbox (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Called when the value of the checkbox is changed, if null does nothing</param>
        public void AddCheckbox(bool defaultValue, string name, Action<bool> onChange = null)
        {
            AddCheckbox(defaultValue, name, out Toggle toggle, onChange);
        }

        /// <summary>
        /// Adds a input field to the mods window
        /// </summary>
        /// <param name="defaultValue">The defualt value before it is edited by the user</param>
        /// <param name="name">Name used both as a display name and as a key for you to get the value by later (no 2 names should EVER be the same)</param>
        /// <param name="inputField">A reference to the created InputField</param>
        /// <param name="onChange">Gets called when the value of the inputField gets changed, if null doesnt nothing</param>
        public void AddInputField(string defaultValue, string name, out InputField inputField, Action<string> onChange = null)
        {
            GameObject InputFieldPrefab = AssetLoader.GetObjectFromFile("modswindow", "InputField", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(InputFieldPrefab);
            spawnedPrefab.transform.parent = _content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = name;
            inputField = spawnedModdedObject.GetObject<InputField>(1);
            inputField.text = defaultValue;

            string loadedString = OptionsSaver.LoadString(_ownerMod, name);
            if (loadedString != null)
            {
                inputField.text = loadedString;
            }

            if (onChange != null)
            {
                onChange(inputField.text);
            }

            inputField.onValueChanged.AddListener(delegate (string value)
            {
                OptionsSaver.SaveString(_ownerMod, name, value);

                if (onChange != null)
                {
                    onChange(value);
                }
            });
        }

        /// <summary>
        /// Adds a input field to the mods window
        /// </summary>
        /// <param name="defaultValue">The defualt value before it is edited by the user</param>
        /// <param name="name">Name used both as a display name and as a key for you to get the value by later (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Gets called when the value of the inputField gets changed, if null doesnt nothing</param>
        public void AddInputField(string defaultValue, string name, Action<string> onChange = null)
        {
            AddInputField(defaultValue, name, out InputField inputField, onChange);
        }


        /// <summary>
        /// Adds a dropdown to the mods window
        /// </summary>
        /// <param name="options">The diffrent options that should be selectable</param>
        /// <param name="defaultIndex">what index in the previus array will be selected before the user edits it</param>
        /// <param name="name">Display name and key for you later (no 2 names should EVER be the same)</param>
        /// <param name="dropdown">a reference to the dropdown created, null if defaultIndex is not in the bounds of options</param>
        /// <param name="onChange">Gets called when the value of the dropdown is changed, if null does nothing</param>
        public void AddDropdown(string[] options, int defaultIndex, string name, out Dropdown dropdown, Action<int> onChange = null)
        {
            if (options.Length <= defaultIndex || defaultIndex < 0)
            {
                dropdown = null;
                return;
            }

            GameObject dropdownPrefab = AssetLoader.GetObjectFromFile("modswindow", "DropDown", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(dropdownPrefab);
            spawnedPrefab.transform.parent = _content.transform;
            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            spawnedModdedObject.GetObject<Text>(0).text = name;

            dropdown = spawnedModdedObject.GetObject<Dropdown>(1);
            dropdown.options.Clear();
            
            foreach (string option in options)
            {
                Dropdown.OptionData data = new Dropdown.OptionData(option);
                dropdown.options.Add(data);
            }
            dropdown.value = defaultIndex;
            dropdown.RefreshShownValue();

            int? loadedInt = OptionsSaver.LoadInt(_ownerMod, name);
            if (loadedInt.HasValue)
            {
                dropdown.value = loadedInt.Value;
                dropdown.RefreshShownValue();
            }

            if (onChange != null)
            {
                onChange(dropdown.value);
            }

            dropdown.onValueChanged.AddListener(delegate (int value)
            {
                OptionsSaver.SaveInt(_ownerMod, name, value);

                if (onChange != null)
                {
                    onChange(value);
                }
            });
        }

        /// <summary>
        /// Adds a dropdown to the mods window
        /// </summary>
        /// <param name="options">The diffrent options that should be selectable</param>
        /// <param name="defaultIndex">what index in the previus array will be selected before the user edits it</param>
        /// <param name="name">Display name and key for you later (no 2 names should EVER be the same)</param>
        /// <param name="onChange">Gets called when the value of the dropdown is changed, if null does nothing</param>
        public void AddDropdown(string[] options, int defaultIndex, string name, Action<int> onChange = null)
        {
            AddDropdown(options, defaultIndex, name, out Dropdown dropdown, onChange);
        }

        /// <summary>
        /// Adds a dropdown to the options window
        /// </summary>
        /// <typeparam name="T">Must be an enum type, the options of this enum will be displayed as the options of the dropdown</typeparam>
        /// <param name="defaultIndex">The index in the enum that will be selected before the user edits it</param>
        /// <param name="name">Display name and key to get value (no 2 names should EVER be the same)</param>
        /// <param name="dropdown">a refrence to the dropdown created</param>
        /// <param name="onChange"></param>
        public void AddDropDown<T>(int defaultIndex, string name, out Dropdown dropdown, Action<int> onChange = null) where T : IComparable, IFormattable, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("The generic type T must be an enum type");
            }

            List<string> enums = EnumTools.GetNames<T>();
            AddDropdown(enums.ToArray(), defaultIndex, name, out dropdown, onChange);
        }


        /// <summary>
        /// Adds a dropdown to the options window
        /// </summary>
        /// <typeparam name="T">Must be an enum type, the options of this enum will be displayed as the options of the dropdown</typeparam>
        /// <param name="defaultIndex">The index in the enum that will be selected before the user edits it</param>
        /// <param name="name">Display name and key to get value (no 2 names should EVER be the same)</param>
        /// <param name="onChange"></param>
        public void AddDropDown<T>(int defaultIndex, string name, Action<int> onChange = null) where T : IComparable, IFormattable, IConvertible
        {
            AddDropDown<T>(defaultIndex, name, out Dropdown dropdown, onChange);
        }

        /// <summary>
        /// Adds a button to the options window
        /// </summary>
        /// <param name="text">The text displayed on the button</param>
        /// <param name="button">a refrence to the created button</param>
        /// <param name="callback">Called when the user clicks the button</param>
        public void AddButton(string text, out Button button, UnityEngine.Events.UnityAction callback)
        {
            GameObject buttonPrefab = AssetLoader.GetObjectFromFile("modswindow", "Button", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(buttonPrefab);
            spawnedPrefab.transform.parent = _content.transform;

            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            button = spawnedModdedObject.GetObject<Button>(0);
            button.onClick.AddListener(callback);
            spawnedModdedObject.GetObject<Text>(1).text = text;
        }

        /// <summary>
        /// Adds a button to the options window
        /// </summary>
        /// <param name="text">The text displayed on the button</param>
        /// <param name="callback">Called when the user clicks the button</param>
        public void AddButton(string text, UnityEngine.Events.UnityAction callback)
        {
            AddButton(text, out Button button, callback);
        }

        /// <summary>
        /// Adds a plain text to the options window
        /// </summary>
        /// <param name="text">string that will be displayed</param>
        /// <param name="_text">a refrence to the created text</param>
        public void AddLabel(string text, out Text _text)
        {
            GameObject labelPrefab = AssetLoader.GetObjectFromFile("modswindow", "Label", "Clone Drone in the Danger Zone_Data/");
            GameObject spawnedPrefab = GameObject.Instantiate(labelPrefab);
            spawnedPrefab.transform.parent = _content.transform;

            ModdedObject spawnedModdedObject = spawnedPrefab.GetComponent<ModdedObject>();
            _text = spawnedModdedObject.GetObject<Text>(0);
            _text.text = text;
        }

        /// <summary>
        /// Adds a plain text to the options window
        /// </summary>
        /// <param name="text">string that will be displayed</param>
        public void AddLabel(string text)
        {
            AddLabel(text, out Text _text);
        }

        /// <summary>
        /// Closes the options window, this also opens its parent window (probably the mods window)
        /// </summary>
        public void CloseWindow()
        {
            RegisterShouldCursorBeEnabledDelegate.UnRegister(shouldCurorBeEnabled);

            GameUIRoot.Instance.SetEscMenuDisabled(false);
            GameObject.Destroy(_spawnedBase);
            _owner.SetActive(true);

            GameUIRoot.Instance.RefreshCursorEnabled();
        }

        /// <summary>
        /// Closes the options window, does NOT open the parent window
        /// </summary>
        public void ForceCloseWindow()
        {
            RegisterShouldCursorBeEnabledDelegate.UnRegister(shouldCurorBeEnabled);

            GameObject.Destroy(_spawnedBase);
            GameUIRoot.Instance.SetEscMenuDisabled(false);

            GameUIRoot.Instance.RefreshCursorEnabled();
        }

        bool shouldCurorBeEnabled()
        {
            return true;
        }
        
    }
}
