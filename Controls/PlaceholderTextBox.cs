using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows text box control with placeholder.
    /// </summary>
    public class PlaceholderTextBox : TextBox
    {
        #region Properties

        string _placeholderText = DEFAULT_PLACEHOLDER;
        bool _isPlaceholderActive = true;


        /// <summary>
        /// Gets a value indicating whether the Placeholder is active.
        /// </summary>
        [Browsable(false)]
        public bool IsPlaceholderActive
        {
            get { return _isPlaceholderActive; }
            private set
            {
                if (_isPlaceholderActive == value) return;

                // Disable operating system handling for mouse events
                // This prevents the user to select the placeholder with mouse or double clicking
                SetStyle(ControlStyles.UserMouse, value);

                _isPlaceholderActive = value;
                OnPlaceholderActiveChanged(value);
            }
        }


        /// <summary>
        /// Gets or sets the placeholder in the PlaceholderTextBox.
        /// </summary>
        [Description("The placeholder associated with the control."), Category("Placeholder"), DefaultValue(DEFAULT_PLACEHOLDER)]
        public string PlaceholderText
        {
            get { return _placeholderText; }
            set
            {
                _placeholderText = value;

                // Only use the new value if the placeholder is active.
                if (IsPlaceholderActive)
                    Text = value;
            }
        }


        /// <summary>
        /// Gets or sets the current text in the TextBox.
        /// </summary>
        [Browsable(false)]
        public override string Text
        {
            // Only overriden to hide it from the designer.
            get
            {
                // Check 'base.Text == Placeholder' because in some cases IsPlaceholderActive changes too late although it isn't the placeholder anymore.
                if (IsPlaceholderActive && base.Text == PlaceholderText)
                    return "";

                return base.Text;
            }
            set { base.Text = value; }
        }



        Color _placeholderTextColor;
        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        [Description("The foreground color of this component, which is used to display the placeholder."), Category("Appearance"), DefaultValue(typeof(Color), "InactiveCaption")]
        public Color PlaceholderTextColor
        {
            get { return _placeholderTextColor; }
            set
            {
                if (_placeholderTextColor == value) return;
                _placeholderTextColor = value;

                // Force redraw to show new color in designer instantly
                if (DesignMode)
                    Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the control.
        /// </summary>
        [Description("The foreground color of this component, which is used to display text."), Category("Appearance"), DefaultValue(typeof(Color), "WindowText")]
        public Color TextColor { get; set; }

        /// <summary>
        /// Do not access directly. Use TextColor.
        /// </summary>
        [Browsable(false)]
        public override Color ForeColor
        {
            get
            {
                if (IsPlaceholderActive)
                    return PlaceholderTextColor;

                return TextColor;
            }
            set { TextColor = value; }
        }


        /// <summary>
        /// Occurs when the value of the IsPlaceholderActive property has changed.
        /// </summary>
        [Description("Occurs when the value of the IsPlaceholderInside property has changed.")]
        public event EventHandler<PlaceholderActiveChangedEventArgs> PlaceholderActiveChanged;

        #endregion


        #region Global Variables

        /// <summary>
        /// Specifies the default placeholder text.
        /// </summary>
        const string DEFAULT_PLACEHOLDER = "<Input>";

        /// <summary>
        /// Flag to avoid the TextChanged Event. Don't access directly, use Method 'ActionWithoutTextChanged(Action act)' instead.
        /// </summary>
        bool avoidTextChanged;

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PlaceholderTextBox class.
        /// </summary>
        public PlaceholderTextBox()
        {
            // Through this line the default placeholder gets displayed in designer
            base.Text = PlaceholderText;
            TextColor = SystemColors.WindowText;
            PlaceholderTextColor = SystemColors.InactiveCaption;

            SetStyle(ControlStyles.UserMouse, true);

            SubscribeEvents();
        }

        #endregion


        #region Functions

        /// <summary>
        /// Inserts placeholder, assigns placeholder style and sets cursor to first position.
        /// </summary>
        public void Reset()
        {
            IsPlaceholderActive = true;

            ActionWithoutTextChanged(() => Text = PlaceholderText);
            Select(0, 0);
        }

        /// <summary>
        /// Run an action with avoiding the TextChanged event.
        /// </summary>
        /// <param name="act">Specifies the action to run.</param>
        private void ActionWithoutTextChanged(Action act)
        {
            avoidTextChanged = true;

            act.Invoke();

            avoidTextChanged = false;
        }

        /// <summary>
        /// Subscribe necessary Events.
        /// </summary>
        private void SubscribeEvents()
        {
            TextChanged += PlaceholderTextBox_TextChanged;
        }

        #endregion


        #region Events

        private void PlaceholderTextBox_TextChanged(object sender, EventArgs e)
        {
            // Check flag
            if (avoidTextChanged) return;

            // Run code with avoiding recursive call
            ActionWithoutTextChanged(delegate
            {
                // If the Text is empty, insert placeholder and set cursor to the first position
                if (String.IsNullOrEmpty(Text))
                {
                    Reset();
                    return;
                }

                // If the placeholder has been active but now the text changed,
                // set the textbox to its usual state
                if (IsPlaceholderActive && Text != PlaceholderText)
                {
                    IsPlaceholderActive = false;

                    // Throw away the placeholder but leave the new typed text
                    int Length = TextLength - PlaceholderText.Length;
                    if (Length > -1)
                        Text = Text.Substring(0, Length);

                    // Set Selection to last position
                    Select(TextLength, 0);
                }
            });
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Prevents that the user can go through the placeholder with arrow keys and placeholder is not deletable with delete key
            if (IsPlaceholderActive && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Delete))
                e.Handled = true;

            base.OnKeyDown(e);
        }

        protected virtual void OnPlaceholderActiveChanged(bool newValue)
        {
            PlaceholderActiveChanged?.Invoke(this, new PlaceholderActiveChangedEventArgs(newValue));
        }

        #endregion


        #region Avoid full text selection after first display with TabIndex = 0

        // Base class has a selectionSet property, but its private.
        // We need to shadow with our own variable. If true, this means
        // "don't mess with the selection, the user did it."
        bool selectionSet;

        protected override void OnGotFocus(EventArgs e)
        {
            bool needToDeselect = false;

            // We don't want to avoid calling the base implementation
            // completely. We mirror the logic that we are trying to avoid;
            // if the base implementation will select all of the text, we
            // set a boolean.
            if (!selectionSet)
            {
                selectionSet = true;

                if (SelectionLength == 0 && MouseButtons == MouseButtons.None)
                {
                    needToDeselect = true;
                }
            }

            // Call the base implementation
            base.OnGotFocus(e);

            // Did we notice that the text was selected automatically? Let's
            // de-select it and put the caret at the end.
            if (!needToDeselect) return;

            SelectionStart = 0;
            DeselectAll();
        }

        #endregion
    }

    /// <summary>
    /// Provides data for the PlaceholderActiveChanged event.
    /// </summary>
    public class PlaceholderActiveChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the PlaceholderActiveChangedEventArgs class.
        /// </summary>
        /// <param name="isActive">Specifies whether the placeholder is currently active.</param>
        public PlaceholderActiveChangedEventArgs(bool isActive)
        {
            IsActive = isActive;
        }

        /// <summary>
        /// Gets the new value of the IsPlaceholderActive property.
        /// </summary>
        public bool IsActive { get; private set; }
    }
}