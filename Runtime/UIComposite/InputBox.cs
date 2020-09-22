﻿using huqiang.Core.HGUI;
using huqiang.Data;
using huqiang.UIEvent;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace huqiang.UIComposite
{
    public class InputBox:Composite
    {
        static float KeySpeed = 220;
        static float MaxSpeed = 30;
        static float KeyPressTime;
        static List<HVertex> hs = new List<HVertex>();
        static List<int> tris = new List<int>();
        Color32 textColor;
        Color32 m_tipColor;
        Color32 PointColor;
        Color32 SelectionColor;
        int CharacterLimit;
        bool ReadOnly;
        string m_TipString;
        string m_InputString;
        public InputType inputType = InputType.Standard;
        public LineType lineType = LineType.SingleLine;
        ContentType m_ctpye;
        bool Editing;
        EmojiString FullString = new EmojiString();
        HImage Caret;
        public object DataContext;
        public string TipString { get { return m_TipString; } set { 
                m_TipString = value;
                SetShowText();
            } }
        public string InputString { get { return FullString.FullString; } set {
                if (value == null)
                    value = "";
                FullString.FullString = value;
                TextOperation.ChangeText(TextCom,FullString);
                SetShowText();
            } }
        public string ShowString { get; private set; }
        public string SelectString { get {
                if (Editing)
                    return TextOperation.GetSelectString();
                return "";
            } }
        public HText TextCom;
        public ContentType contentType
        {
            get { return m_ctpye; }
            set
            {
                m_ctpye = value;
                switch (value)
                {
                    case ContentType.Standard:
                        {
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.Autocorrected:
                        {
                            inputType = InputType.AutoCorrect;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.IntegerNumber:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NumberPad;
                            characterValidation = CharacterValidation.Integer;
                            break;
                        }
                    case ContentType.DecimalNumber:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NumbersAndPunctuation;
                            characterValidation = CharacterValidation.Decimal;
                            break;
                        }
                    case ContentType.Alphanumeric:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.ASCIICapable;
                            characterValidation = CharacterValidation.Alphanumeric;
                            break;
                        }
                    case ContentType.Name:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NamePhonePad;
                            characterValidation = CharacterValidation.Name;
                            break;
                        }
                    case ContentType.NumberAndName:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.NamePhonePad;
                            characterValidation = CharacterValidation.numberAndName;
                            break;
                        }
                    case ContentType.EmailAddress:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Standard;
                            touchType = TouchScreenKeyboardType.EmailAddress;
                            characterValidation = CharacterValidation.EmailAddress;
                            break;
                        }
                    case ContentType.Password:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Password;
                            touchType = TouchScreenKeyboardType.Default;
                            characterValidation = CharacterValidation.None;
                            break;
                        }
                    case ContentType.Pin:
                        {
                            lineType = LineType.SingleLine;
                            inputType = InputType.Password;
                            touchType = TouchScreenKeyboardType.NumberPad;
                            characterValidation = CharacterValidation.Integer;
                            break;
                        }
                    default:
                        {
                            // Includes Custom type. Nothing should be enforced.
                            break;
                        }
                }
            }
        }
        CharacterValidation characterValidation = CharacterValidation.None;
        TouchScreenKeyboardType touchType = TouchScreenKeyboardType.Default;
        public InputBoxEvent InputEvent;
        public Action<InputBox> OnSubmit;
        public Action<InputBox> OnDone;
        public Action<InputBox> OnValueChanged;
        public Func<InputBox, int, char, char> ValidateChar;
        public override void Initial(FakeStruct mod, UIElement element)
        {
            base.Initial(mod,element);
            var txt = TextCom = element.GetComponentInChildren<HText>();
            textColor = txt.m_color;
            unsafe
            {
                var ex = mod.buffer.GetData(((TransfromData*)mod.ip)->ex) as FakeStruct;
                if (ex != null)
                {
                    TextInputData* tp = (TextInputData*)ex.ip;
                    textColor = tp->inputColor;
                    m_tipColor = tp->tipColor;
                    PointColor = tp->pointColor;
                    SelectionColor = tp->selectColor;
                    CharacterLimit = tp->CharacterLimit;
                    ReadOnly = tp->ReadyOnly;
                    contentType = tp->contentType;
                    lineType = tp->lineType;
                    m_TipString = mod.buffer.GetData(tp->tipString) as string;
                    m_InputString = mod.buffer.GetData(tp->inputString) as string;
                }
                else
                {
                    m_InputString = txt.Text;
                }
            }
            FullString.FullString = m_InputString;
            InputEvent = txt.RegEvent<InputBoxEvent>();
            InputEvent.input = this;
            Caret = txt.GetComponentInChildren<HImage>();
        }
        public void OnMouseDown(UserAction action, ref PressInfo press)
        {
            TextOperation.ChangeText(TextCom, FullString);
            SetShowText();
            TextOperation.SetPress(ref press);
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            if (!ReadOnly)
                Editing = true;
#endif
        }
        public void OnClick(UserAction action)
        {
            if (!ReadOnly)
            {
                Editing = true;
                if (!Keyboard.active)
                {
                    bool pass = contentType == ContentType.Password ? true : false;
                    bool multiLine = lineType == LineType.MultiLineNewline ? true : false;
                    Keyboard.OnInput(FullString.FullString, touchType, multiLine, pass, CharacterLimit);
                }
            }
        }
        public void OnLostFocus(UserAction action)
        {
            Editing = false;
            TextCom.Text = FullString.FullString;
            if (ReplaceTarget != null)
            {
                ReplaceTarget.Text = FullString.FullString;
                Enity.gameObject.SetActive(false);
            }
            else SetShowText();
            if (OnDone != null)
                OnDone(this);
        }
        public void OnDrag(UserAction action, ref PressInfo press)
        {
            TextOperation.SetEndPress(ref press);
        }
        string OnInputChanged(string input)
        {
            if (ReadOnly)
                return "";
            if (input == null | input == "")
                return "";
            EmojiString es = new EmojiString(input);
            string str = FullString.FilterString;
            if (CharacterLimit > 0)
            {
                string fs = es.FilterString;
                if (fs.Length + str.Length > CharacterLimit)
                {
                    int len = CharacterLimit - str.Length;
                    if (len <= 0)
                        return "";
                    es.Remove(fs.Length - len, len);
                }
            }
            str = es.FullString;
            int s = TextOperation.StartPress.Index;
            if (CharOperation.Validate(characterValidation, str, s, str[0]) == 0)
                return "";
            if (ValidateChar != null)
                if (ValidateChar(this, s, str[0]) == 0)
                    return "";
            InsertString(str);
            return input;
        }
        void TouchInputChanged(string input)
        {
            if (Keyboard.InputChanged)
            {
                FullString.FullString = input;
                TextOperation.ChangeText(TextCom,FullString);
                var sel = Keyboard.selection;
                int start = sel.start;
                int end = sel.start + sel.length;
                if (end != start)
                {
                    TextOperation.SetStartPressIndex(start);
                    TextOperation.SetEndPressIndex(end);
                }
                else 
                {
                    bool lc = false;
                    TextOperation.SetPressIndex(start,ref lc); 
                }
                SetShowText();
            }
            if (OnValueChanged != null)
                OnValueChanged(this);
        }
        public void SetShowText()
        {
            var str = FullString.FullString;
            if (Editing)
            {
                if (TextCom == null)
                    return;
                str = TextOperation.GetShowContent();//GetShowString();
                TextCom.MainColor = textColor;
                if (contentType == ContentType.Password)
                    TextCom.Text = new string('*', str.Length);
                else TextCom.Text = str;
                InputEvent.ChangeText(str);
                ShowString = str;
            }
            else if (str != "" & str != null)
            {
                if (TextCom == null)
                    return;
                TextCom.MainColor = textColor;
                if (contentType == ContentType.Password)
                    TextCom.Text = new string('*', str.Length);
                else TextCom.Text = str;
                ShowString = str;
            }
            else
            {
                TextCom.MainColor = m_tipColor;
                TextCom.Text = m_TipString;
                ShowString = m_TipString;
            }
        }
        public bool DeleteSelectString()
        {
            if (TextOperation.DeleteSelectString())
            {
                SetShowText();
                return true;
            }
            return false;
        }
        public bool DeleteLast()
        {
            if (TextOperation.DeleteLast())
            {
                SetShowText();
                return true;
            }
            return false;
        }
        public bool DeleteNext()
        {
            if (TextOperation.DeleteNext())
            {
                SetShowText();
                return true;
            }
            return false;
        }
        public void InsertString(string str)
        {
            var es = new EmojiString(str);
            TextOperation.DeleteSelectString();
            TextOperation.InsertContent(es);
            SetShowText();
        }
        public void PointerMoveLeft()
        {
            bool lc = false;
            if (TextOperation.PointerMoveLeft(ref lc))
            {
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveRight()
        {
            bool lc = false;
            if (TextOperation.PointerMoveRight(ref lc))
            {
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveUp()
        {
            bool lc = false;
            if (TextOperation.PointerMoveUp(ref lc))
            {
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveDown()
        {
            bool lc = false;
            if (TextOperation.PointerMoveDown(ref lc))
            {
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveStart()
        {
            bool lc = false;
            if (TextOperation.SetPressIndex(0, ref lc))
            {
                if (lc)
                    SetShowText();
            }
        }
        public void PointerMoveEnd()
        {
            bool lc = false;
            if (TextOperation.SetPressIndex(999999999, ref lc))
            {
                if (lc)
                    SetShowText();
            }
        }
        public override void Update(float time)
        {
            if (Editing)
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                var state = KeyPressed();
                if (state == EditState.Continue)
                {
                    if (Keyboard.InputChanged)
                    {
                        OnInputChanged(Keyboard.InputString);
                    }
                    else if (Keyboard.TempStringChanged)
                    {
                        SetShowText();
                    }
                }
                else if (state == EditState.Finish)
                {
                    if (!ReadOnly)
                        if (OnSubmit != null)
                            OnSubmit(this);
                }
                else if (state == EditState.NewLine)
                {
                    if(!ReadOnly)
                    {
                        if (lineType == LineType.SingleLine)
                        {
                            if (OnSubmit != null)
                                OnSubmit(this);
                        }
                        else OnInputChanged("\r\n");
                    }
                }
#else
                        TouchInputChanged(Keyboard.TouchString);
                        if (Keyboard.status == TouchScreenKeyboard.Status.Done)
                        {
                            if (OnDone!= null)
                                OnDone(this);
                            return;
                        }
#endif
                UpdateCaret();
            }
            else
            {
                if (Caret != null)
                    Caret.gameObject.SetActive(false);
            }
        }
        float time;
        void UpdateCaret()
        {
            if (Caret != null)
            {
                hs.Clear();
                tris.Clear();
                switch (TextOperation.Style)
                {
                    case 1:
                        if (!ReadOnly)
                        {
                            time += UserAction.TimeSlice;
                            if (time > 1000f)
                            {
                                time = 0;
                            }
                            else if (time > 400f)
                            {
                                Caret.gameObject.SetActive(false);
                            }
                            else
                            {
                                Caret.gameObject.SetActive(true);
                                PressInfo start = TextOperation.GetStartPress();
                                InputEvent.GetPointer(tris, hs, ref PointColor, ref start);
                                Caret.LoadFromMesh(hs, tris);
                                PressInfo end = TextOperation.GetEndPress();
                                InputEvent.SetCursorPos(ref end);
                            }
                        }
                        else Caret.gameObject.SetActive(false);
                        break;
                    case 2:
                        Caret.gameObject.SetActive(true);
                        PressInfo s = TextOperation.GetStartPress();
                        PressInfo e = TextOperation.GetEndPress();
                        InputEvent.GetSelectArea(tris, hs, ref SelectionColor, ref s,ref e);
                        Caret.LoadFromMesh(hs, tris);
                        break;
                    default:
                        Caret.gameObject.SetActive(false);
                        break;
                }
            }
        }
        void KeySpeedUp()
        {
            KeySpeed *= 0.7f;
            if (KeySpeed < MaxSpeed)
                KeySpeed = MaxSpeed;
            KeyPressTime = KeySpeed;
        }
        EditState KeyPressed()
        {
            KeyPressTime -= UserAction.TimeSlice;
            if (Keyboard.GetKey(KeyCode.Backspace))
            {
                if (!ReadOnly)
                    if (KeyPressTime <= 0)
                    {
                        DeleteLast();
                        KeySpeedUp();
                    }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.Delete))
            {
                if (!ReadOnly)
                    if (KeyPressTime <= 0)
                    {
                        DeleteNext();
                        KeySpeedUp();
                    }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.LeftArrow))
            {
                if (KeyPressTime <= 0)
                {
                    PointerMoveLeft();
                    KeySpeedUp();
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.RightArrow))
            {
                if (KeyPressTime <= 0)
                {
                    PointerMoveRight();
                    KeySpeedUp();
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.UpArrow))
            {
                if (KeyPressTime <= 0)
                {
                    PointerMoveUp();
                    KeySpeedUp();
                }
                return EditState.Done;
            }
            if (Keyboard.GetKey(KeyCode.DownArrow))
            {
                if (KeyPressTime <= 0)
                {
                    PointerMoveDown();
                    KeySpeedUp();
                }
                return EditState.Done;
            }
            KeySpeed = 220f;
            if (Keyboard.GetKeyDown(KeyCode.Home))
            {
                PointerMoveStart();
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.End))
            {
                PointerMoveEnd();
                return EditState.Done;
            }
            if (Keyboard.GetKeyDown(KeyCode.A))
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    TextOperation.SelectAll();
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.X))//剪切
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    string str = TextOperation.GetSelectString();
                    if (!ReadOnly)
                        DeleteSelectString();
                    GUIUtility.systemCopyBuffer = str;
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.C))//复制
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    string str = TextOperation.GetSelectString();
                    GUIUtility.systemCopyBuffer = str;
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.V))//粘贴
            {
                if (Keyboard.GetKey(KeyCode.LeftControl) | Keyboard.GetKey(KeyCode.RightControl))
                {
                    OnInputChanged(Keyboard.systemCopyBuffer);
                    return EditState.Done;
                }
            }
            if (Keyboard.GetKeyDown(KeyCode.Return) | Keyboard.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (lineType == LineType.MultiLineNewline)
                {
                    if (Keyboard.GetKey(KeyCode.RightControl))
                        return EditState.Finish;
                    return EditState.NewLine;
                }
                else return EditState.Finish;
            }
            if (Keyboard.GetKeyDown(KeyCode.Escape))
            {
                return EditState.Finish;
            }
            return EditState.Continue;
        }
        public HText ReplaceTarget { get; private set; }
        public void Replace(HText text, UserEvent user, UserAction action)
        {
            ReplaceTarget = text;
            if (text == null)
            {
                return;
            }
            var son = Enity.transform;
            var par = text.transform;
            son.SetParent(par);
            son.localPosition = Vector3.zero;
            son.localScale = Vector3.one;
            son.localRotation = Quaternion.identity;
            Enity.marginType = MarginType.Margin;
            Enity.margin.left = 0;
            Enity.margin.right = 0;
            Enity.margin.top = 0;
            Enity.margin.down = 0;
            UIElement.Resize(Enity);
            HTextLoader.CopyTo(text,TextCom);
            FullString.FullString = text.Text;
            TextOperation.ChangeText(TextCom,FullString);
            action.AddFocus(InputEvent);
            Enity.gameObject.SetActive(true);
            text.Text = "";
            Editing = true;
            SetShowText();
            PressInfo press = new PressInfo();
            InputEvent.GlobalPosition = user.GlobalPosition;
            InputEvent.GlobalScale = user.GlobalScale;
            InputEvent.GlobalRotation = user.GlobalRotation;
            InputEvent.CheckPointer(action,ref press);
            TextOperation.SetPress(ref press);
        }
    }
}
