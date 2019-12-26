// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
//
// Notes:
//

using UnityEngine;
using System;
using System.Collections.Generic;
using DaggerfallConnect.Utility;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Spellmaker UI.
    /// </summary>
    public class DaggerfallSpellMakerWindow : DaggerfallPopupWindow
    {
        #region UI Rects

        DFSize selectedIconsBaseSize = new DFSize(40, 80);

        Vector2 tipLabelPos = new Vector2(5, 22);
        Vector2 nameLabelPos = new Vector2(60, 185);
        Rect effect1NameRect = new Rect(3, 30, 230, 9);
        Rect effect2NameRect = new Rect(3, 62, 230, 9);
        Rect effect3NameRect = new Rect(3, 94, 230, 9);
        Rect addEffectButtonRect = new Rect(244, 114, 28, 28);
        Rect buyButtonRect = new Rect(244, 147, 24, 16);
        Rect newButtonRect = new Rect(244, 163, 24, 16);
        Rect exitButtonRect = new Rect(244, 179, 24, 16);
        Rect casterOnlyButtonRect = new Rect(275, 114, 24, 16);
        Rect byTouchButtonRect = new Rect(275, 130, 24, 16);
        Rect singleTargetAtRangeButtonRect = new Rect(275, 146, 24, 16);
        Rect areaAroundCasterButtonRect = new Rect(275, 162, 24, 16);
        Rect areaAtRangeButtonRect = new Rect(275, 178, 24, 16);
        Rect fireBasedButtonRect = new Rect(299, 114, 16, 16);
        Rect coldBasedButtonRect = new Rect(299, 130, 16, 16);
        Rect poisonBasedButtonRect = new Rect(299, 146, 16, 16);
        Rect shockBasedButtonRect = new Rect(299, 162, 16, 16);
        Rect magicBasedButtonRect = new Rect(299, 178, 16, 16);
        Rect nextIconButtonRect = new Rect(275, 80, 9, 16);
        Rect previousIconButtonRect = new Rect(275, 96, 9, 16);
        Rect selectIconButtonRect = new Rect(288, 94, 16, 16);
        Rect nameSpellButtonRect = new Rect(59, 184, 142, 7);

        Rect casterOnlySubRect = new Rect(0, 0, 24, 16);
        Rect byTouchSubRect = new Rect(0, 16, 24, 16);
        Rect singleTargetAtRangeSubRect = new Rect(0, 32, 24, 16);
        Rect areaAroundCasterSubRect = new Rect(0, 48, 24, 16);
        Rect areaAtRangeSubRect = new Rect(0, 64, 24, 16);

        Rect fireBasedSubRect = new Rect(24, 0, 16, 16);
        Rect coldBasedSubRect = new Rect(24, 16, 16, 16);
        Rect poisonBasedSubRect = new Rect(24, 32, 16, 16);
        Rect shockBasedSubRect = new Rect(24, 48, 16, 16);
        Rect magicBasedSubRect = new Rect(24, 64, 16, 16);

        #endregion

        #region UI Controls

        TextLabel tipLabel;
        TextLabel effect1NameLabel;
        TextLabel effect2NameLabel;
        TextLabel effect3NameLabel;
        TextLabel maxSpellPointsLabel;
        TextLabel moneyLabel;
        TextLabel goldCostLabel;
        TextLabel spellPointCostLabel;
        TextLabel spellNameLabel;

        DaggerfallListPickerWindow effectGroupPicker;
        DaggerfallListPickerWindow effectSubGroupPicker;
        DaggerfallEffectSettingsEditorWindow effectEditor;
        SpellIconPickerWindow iconPicker;

        Button selectIconButton;

        Button casterOnlyButton;
        Button byTouchButton;
        Button singleTargetAtRangeButton;
        Button areaAroundCasterButton;
        Button areaAtRangeButton;

        Button fireBasedButton;
        Button coldBasedButton;
        Button poisonBasedButton;
        Button shockBasedButton;
        Button magicBasedButton;

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        Texture2D selectedIconsTexture;

        Texture2D casterOnlySelectedTexture;
        Texture2D byTouchSelectedTexture;
        Texture2D singleTargetAtRangeSelectedTexture;
        Texture2D areaAroundCasterSelectedTexture;
        Texture2D areaAtRangeSelectedTexture;

        Texture2D fireBasedSelectedTexture;
        Texture2D coldBasedSelectedTexture;
        Texture2D poisonBasedSelectedTexture;
        Texture2D shockBasedSelectedTexture;
        Texture2D magicBasedSelectedTexture;

        #endregion

        #region Fields

        const MagicCraftingStations thisMagicStation = MagicCraftingStations.SpellMaker;

        const string textDatabase = "SpellmakerUI";
        const string baseTextureFilename = "INFO01I0.IMG";
        const string goldSelectIconsFilename = "MASK01I0.IMG";
        const string colorSelectIconsFilename = "MASK04I0.IMG";

        const int alternateAlphaIndex = 12;
        const int maxEffectsPerSpell = 3;
        const int defaultSpellIcon = 1;
        const TargetTypes defaultTargetFlags = EntityEffectBroker.TargetFlags_All;
        const ElementTypes defaultElementFlags = EntityEffectBroker.ElementFlags_MagicOnly;

        const SoundClips inscribeGrimoire = SoundClips.ParchmentScratching;

        List<IEntityEffect> enumeratedEffectTemplates = new List<IEntityEffect>();

        EffectEntry[] effectEntries = new EffectEntry[maxEffectsPerSpell];

        int editOrDeleteSlot = -1;
        TargetTypes allowedTargets = defaultTargetFlags;
        ElementTypes allowedElements = defaultElementFlags;
        TargetTypes selectedTarget = TargetTypes.CasterOnly;
        ElementTypes selectedElement = ElementTypes.Magic;
        SpellIcon selectedIcon;

        int totalGoldCost = 0;
        int totalSpellPointCost = 0;

        #endregion

        #region Constructors

        public DaggerfallSpellMakerWindow(IUserInterfaceManager uiManager, DaggerfallBaseWindow previous = null)
            : base(uiManager, previous)
        {
        }

        #endregion

        #region Setup Methods

        protected override void Setup()
        {
            // Load all the textures used by spell maker window
            LoadTextures();

            // Always dim background
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundColor = new Color(0, 0, 0, 0.75f);
            NativePanel.BackgroundTexture = baseTexture;

            // Setup controls
            SetupLabels();
            SetupButtons();
            SetupPickers();
            SetIcon(selectedIcon);
            SetStatusLabels();

            // Setup effect editor window
            effectEditor = new DaggerfallEffectSettingsEditorWindow(uiManager, this);
            effectEditor.OnSettingsChanged += EffectEditor_OnSettingsChanged;
            effectEditor.OnClose += EffectEditor_OnClose;

            // Setup icon picker
            iconPicker = new SpellIconPickerWindow(uiManager, this);
            iconPicker.OnClose += IconPicker_OnClose;
        }

        public override void OnPush()
        {
            InitEffectSlots();
            
            SetDefaults();
        }

        void SetDefaults()
        {
            allowedTargets = defaultTargetFlags;
            allowedElements = defaultElementFlags;
            selectedIcon = new SpellIcon()
            {
                index = defaultSpellIcon,
            };
            editOrDeleteSlot = -1;

            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                effectEntries[i] = new EffectEntry();
            }

            if (IsSetup)
            {
                effect1NameLabel.Text = string.Empty;
                effect2NameLabel.Text = string.Empty;
                effect3NameLabel.Text = string.Empty;
                spellNameLabel.Text = string.Empty;
                UpdateAllowedButtons();
                SetIcon(selectedIcon);
                SetStatusLabels();
            }
        }

        void SetStatusLabels()
        {
            maxSpellPointsLabel.Text = GameManager.Instance.PlayerEntity.MaxMagicka.ToString();
            moneyLabel.Text = GameManager.Instance.PlayerEntity.GoldPieces.ToString();
            goldCostLabel.Text = totalGoldCost.ToString();
            spellPointCostLabel.Text = totalSpellPointCost.ToString();
        }

        #endregion

        #region Private Methods

        void LoadTextures()
        {
            // Load source textures
            baseTexture = ImageReader.GetTexture(baseTextureFilename, 0, 0, true, alternateAlphaIndex);
            selectedIconsTexture = ImageReader.GetTexture(goldSelectIconsFilename);

            // Load target texture
            casterOnlySelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, casterOnlySubRect, selectedIconsBaseSize);
            byTouchSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, byTouchSubRect, selectedIconsBaseSize);
            singleTargetAtRangeSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, singleTargetAtRangeSubRect, selectedIconsBaseSize);
            areaAroundCasterSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, areaAroundCasterSubRect, selectedIconsBaseSize);
            areaAtRangeSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, areaAtRangeSubRect, selectedIconsBaseSize);

            fireBasedSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, fireBasedSubRect, selectedIconsBaseSize);
            coldBasedSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, coldBasedSubRect, selectedIconsBaseSize);
            poisonBasedSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, poisonBasedSubRect, selectedIconsBaseSize);
            shockBasedSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, shockBasedSubRect, selectedIconsBaseSize);
            magicBasedSelectedTexture = ImageReader.GetSubTexture(selectedIconsTexture, magicBasedSubRect, selectedIconsBaseSize);
        }

        void SetupLabels()
        {
            // Tip label
            tipLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, tipLabelPos, string.Empty, NativePanel);

            // Status labels
            maxSpellPointsLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(43, 149), string.Empty, NativePanel);
            moneyLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(39, 158), string.Empty, NativePanel);
            goldCostLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(59, 167), string.Empty, NativePanel);
            spellPointCostLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(70, 176), string.Empty, NativePanel);

            // Name label
            spellNameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, nameLabelPos, string.Empty, NativePanel);
            spellNameLabel.ShadowPosition = Vector2.zero;

            // Effect1
            Panel effect1NamePanel = DaggerfallUI.AddPanel(effect1NameRect, NativePanel);
            effect1NamePanel.HorizontalAlignment = HorizontalAlignment.Center;
            effect1NamePanel.OnMouseClick += Effect1NamePanel_OnMouseClick;
            effect1NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect1NamePanel);
            effect1NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect1NameLabel.ShadowPosition = Vector2.zero;

            // Effect2
            Panel effect2NamePanel = DaggerfallUI.AddPanel(effect2NameRect, NativePanel);
            effect2NamePanel.HorizontalAlignment = HorizontalAlignment.Center;
            effect2NamePanel.OnMouseClick += Effect2NamePanel_OnMouseClick;
            effect2NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect2NamePanel);
            effect2NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect2NameLabel.ShadowPosition = Vector2.zero;

            // Effect3
            Panel effect3NamePanel = DaggerfallUI.AddPanel(effect3NameRect, NativePanel);
            effect3NamePanel.HorizontalAlignment = HorizontalAlignment.Center;
            effect3NamePanel.OnMouseClick += Effect3NamePanel_OnMouseClick;
            effect3NameLabel = DaggerfallUI.AddTextLabel(DaggerfallUI.LargeFont, Vector2.zero, string.Empty, effect3NamePanel);
            effect3NameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            effect3NameLabel.ShadowPosition = Vector2.zero;
        }

        void SetupPickers()
        {
            // Use a picker for effect group
            effectGroupPicker = new DaggerfallListPickerWindow(uiManager, this);
            effectGroupPicker.ListBox.OnUseSelectedItem += AddEffectGroupListBox_OnUseSelectedItem;

            // Use another picker for effect subgroup
            // This allows user to hit escape and return to effect group list, unlike classic which dumps whole spellmaker UI
            effectSubGroupPicker = new DaggerfallListPickerWindow(uiManager, this);
            effectSubGroupPicker.ListBox.OnUseSelectedItem += AddEffectSubGroup_OnUseSelectedItem;
        }

        void SetupButtons()
        {
            // Control
            AddTipButton(addEffectButtonRect, "addEffect", AddEffectButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerAddEffect);
            AddTipButton(buyButtonRect, "buySpell", BuyButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerBuySpell);
            AddTipButton(newButtonRect, "newSpell", NewSpellButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerNewSpell);
            AddTipButton(exitButtonRect, "exit", ExitButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerExit);
            AddTipButton(nameSpellButtonRect, "nameSpell", NameSpellButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerNameSpell);

            // Target
            casterOnlyButton = AddTipButton(casterOnlyButtonRect, "casterOnly", CasterOnlyButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerTargetCaster);
            byTouchButton = AddTipButton(byTouchButtonRect, "byTouch", ByTouchButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerTargetTouch);
            singleTargetAtRangeButton = AddTipButton(singleTargetAtRangeButtonRect, "singleTargetAtRange", SingleTargetAtRangeButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerTargetSingleAtRange);
            areaAroundCasterButton = AddTipButton(areaAroundCasterButtonRect, "areaAroundCaster", AreaAroundCasterButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerTargetAroundCaster);
            areaAtRangeButton = AddTipButton(areaAtRangeButtonRect, "areaAtRange", AreaAtRangeButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerTargetAreaAtRange);

            // Element
            fireBasedButton = AddTipButton(fireBasedButtonRect, "fireBased", FireBasedButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerElementFire);
            coldBasedButton = AddTipButton(coldBasedButtonRect, "coldBased", ColdBasedButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerElementCold);
            poisonBasedButton = AddTipButton(poisonBasedButtonRect, "poisonBased", PoisonBasedButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerElementPoison);
            shockBasedButton = AddTipButton(shockBasedButtonRect, "shockBased", ShockBasedButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerElementShock);
            magicBasedButton = AddTipButton(magicBasedButtonRect, "magicBased", MagicBasedButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerElementMagic);

            // Icons
            AddTipButton(nextIconButtonRect, "nextIcon", NextIconButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerNextIcon);
            AddTipButton(previousIconButtonRect, "previousIcon", PreviousIconButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerPrevIcon);
            selectIconButton = AddTipButton(selectIconButtonRect, "selectIcon", SelectIconButton_OnMouseClick, DaggerfallShortcut.Buttons.SpellMakerSelectIcon);
            //selectIconButton.OnRightMouseClick += PreviousIconButton_OnMouseClick;

            // Select default buttons
            UpdateAllowedButtons();
        }

        Button AddTipButton(Rect rect, string tipID, BaseScreenComponent.OnMouseClickHandler handler, DaggerfallShortcut.Buttons button)
        {
            Button tipButton = DaggerfallUI.AddButton(rect, NativePanel);
            tipButton.OnMouseEnter += TipButton_OnMouseEnter;
            tipButton.OnMouseLeave += TipButton_OnMouseLeave;
            tipButton.OnMouseClick += handler;
            tipButton.Hotkey = DaggerfallShortcut.GetBinding(button);
            tipButton.Tag = tipID;

            return tipButton;
        }

        void InitEffectSlots()
        {
            effectEntries = new EffectEntry[maxEffectsPerSpell];
            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                effectEntries[i] = new EffectEntry();
            }
        }

        void ClearPendingDeleteEffectSlot()
        {
            if (editOrDeleteSlot == -1)
                return;

            effectEntries[editOrDeleteSlot] = new EffectEntry();
            UpdateSlotText(editOrDeleteSlot, string.Empty);
            editOrDeleteSlot = -1;
            UpdateAllowedButtons();
        }

        int GetFirstFreeEffectSlotIndex()
        {
            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                if (string.IsNullOrEmpty(effectEntries[i].Key))
                    return i;
            }

            return -1;
        }

        int GetFirstUsedEffectSlotIndex()
        {
            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                if (!string.IsNullOrEmpty(effectEntries[i].Key))
                    return i;
            }

            return -1;
        }

        int CountUsedEffectSlots()
        {
            int total = 0;
            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                if (!string.IsNullOrEmpty(effectEntries[i].Key))
                    total++;
            }

            return total;
        }

        void UpdateSlotText(int slot, string text)
        {
            // Get text label to update
            TextLabel label = null;
            switch (slot)
            {
                case 0:
                    label = effect1NameLabel;
                    break;
                case 1:
                    label = effect2NameLabel;
                    break;
                case 2:
                    label = effect3NameLabel;
                    break;
                default:
                    return;
            }

            // Set label text
            label.Text = text;
        }

        void AddAndEditSlot(IEntityEffect effectTemplate)
        {
            effectEditor.EffectTemplate = effectTemplate;
            int slot = GetFirstFreeEffectSlotIndex();
            effectEntries[slot] = effectEditor.EffectEntry;
            UpdateSlotText(slot, effectEditor.EffectTemplate.DisplayName);
            UpdateAllowedButtons();
            editOrDeleteSlot = slot;
            uiManager.PushWindow(effectEditor);
        }

        void EditOrDeleteSlot(int slot)
        {
            const int howToAlterSpell = 1708;

            // Do nothing if slot not set
            if (string.IsNullOrEmpty(effectEntries[slot].Key))
                return;

            // Offer to edit or delete effect
            editOrDeleteSlot = slot;
            DaggerfallMessageBox mb = new DaggerfallMessageBox(uiManager, this);
            mb.SetTextTokens(howToAlterSpell);
            Button editButton = mb.AddButton(DaggerfallMessageBox.MessageBoxButtons.Edit);
            editButton.OnMouseClick += EditButton_OnMouseClick;
            Button deleteButton = mb.AddButton(DaggerfallMessageBox.MessageBoxButtons.Delete);
            deleteButton.OnMouseClick += DeleteButton_OnMouseClick;
            mb.OnButtonClick += EditOrDeleteSpell_OnButtonClick;
            mb.OnCancel += EditOrDeleteSpell_OnCancel;
            mb.Show();
        }

        void SetSpellTarget(TargetTypes targetType)
        {
            // Exclude target types based on effects added
            if ((allowedTargets & targetType) == TargetTypes.None)
                return;

            // Clear buttons
            casterOnlyButton.BackgroundTexture = null;
            byTouchButton.BackgroundTexture = null;
            singleTargetAtRangeButton.BackgroundTexture = null;
            areaAroundCasterButton.BackgroundTexture = null;
            areaAtRangeButton.BackgroundTexture = null;

            // Set selected icon
            switch (targetType)
            {
                case TargetTypes.CasterOnly:
                    casterOnlyButton.BackgroundTexture = casterOnlySelectedTexture;
                    break;
                case TargetTypes.ByTouch:
                    byTouchButton.BackgroundTexture = byTouchSelectedTexture;
                    break;
                case TargetTypes.SingleTargetAtRange:
                    singleTargetAtRangeButton.BackgroundTexture = singleTargetAtRangeSelectedTexture;
                    break;
                case TargetTypes.AreaAroundCaster:
                    areaAroundCasterButton.BackgroundTexture = areaAroundCasterSelectedTexture;
                    break;
                case TargetTypes.AreaAtRange:
                    areaAtRangeButton.BackgroundTexture = areaAtRangeSelectedTexture;
                    break;
            }

            selectedTarget = targetType;
            UpdateSpellCosts();
        }

        void SetSpellElement(ElementTypes elementType)
        {
            // Exclude element types based on effects added
            if ((allowedElements & elementType) == ElementTypes.None)
                return;

            // Clear buttons
            fireBasedButton.BackgroundTexture = null;
            coldBasedButton.BackgroundTexture = null;
            poisonBasedButton.BackgroundTexture = null;
            shockBasedButton.BackgroundTexture = null;
            magicBasedButton.BackgroundTexture = null;

            // Set selected icon
            switch (elementType)
            {
                case ElementTypes.Fire:
                    fireBasedButton.BackgroundTexture = fireBasedSelectedTexture;
                    break;
                case ElementTypes.Cold:
                    coldBasedButton.BackgroundTexture = coldBasedSelectedTexture;
                    break;
                case ElementTypes.Poison:
                    poisonBasedButton.BackgroundTexture = poisonBasedSelectedTexture;
                    break;
                case ElementTypes.Shock:
                    shockBasedButton.BackgroundTexture = shockBasedSelectedTexture;
                    break;
                case ElementTypes.Magic:
                    magicBasedButton.BackgroundTexture = magicBasedSelectedTexture;
                    break;
            }

            selectedElement = elementType;
        }

        void UpdateAllowedButtons()
        {
            // Set defaults when no effects added
            if (GetFirstUsedEffectSlotIndex() == -1)
            {
                allowedTargets = defaultTargetFlags;
                allowedElements = defaultElementFlags;
                SetSpellTarget(TargetTypes.CasterOnly);
                SetSpellElement(ElementTypes.Magic);
                EnforceSelectedButtons();
                return;
            }

            // Combine flags
            allowedTargets = EntityEffectBroker.TargetFlags_All;
            allowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                // Must be a valid entry
                if (!string.IsNullOrEmpty(effectEntries[i].Key))
                {
                    // Get effect template
                    IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(effectEntries[i].Key);

                    // Allowed targets are least permissive result set from combined target flags
                    allowedTargets = allowedTargets & effectTemplate.Properties.AllowedTargets;

                    // Allowed elements are most permissive result set from combined element flags (magic always allowed)
                    allowedElements = allowedElements | effectTemplate.Properties.AllowedElements;
                }
            }

            // Ensure a valid button is selected
            EnforceSelectedButtons();
        }

        void EnforceSelectedButtons()
        {
            if ((allowedTargets & selectedTarget) == TargetTypes.None)
                SelectFirstAllowedTargetType();

            if ((allowedElements & selectedElement) == ElementTypes.None)
                SelectFirstAllowedElementType();
        }

        void SelectFirstAllowedTargetType()
        {
            if ((allowedTargets & TargetTypes.CasterOnly) == TargetTypes.CasterOnly)
            {
                SetSpellTarget(TargetTypes.CasterOnly);
                return;
            }
            else if ((allowedTargets & TargetTypes.ByTouch) == TargetTypes.ByTouch)
            {
                SetSpellTarget(TargetTypes.ByTouch);
                return;
            }
            else if ((allowedTargets & TargetTypes.SingleTargetAtRange) == TargetTypes.SingleTargetAtRange)
            {
                SetSpellTarget(TargetTypes.SingleTargetAtRange);
                return;
            }
            else if ((allowedTargets & TargetTypes.AreaAroundCaster) == TargetTypes.AreaAroundCaster)
            {
                SetSpellTarget(TargetTypes.AreaAroundCaster);
                return;
            }
            else if ((allowedTargets & TargetTypes.AreaAtRange) == TargetTypes.AreaAtRange)
            {
                SetSpellTarget(TargetTypes.AreaAtRange);
                return;
            }
        }

        void SelectFirstAllowedElementType()
        {
            if ((allowedElements & ElementTypes.Fire) == ElementTypes.Fire)
            {
                SetSpellElement(ElementTypes.Fire);
                return;
            }
            else if ((allowedElements & ElementTypes.Cold) == ElementTypes.Cold)
            {
                SetSpellElement(ElementTypes.Cold);
                return;
            }
            else if ((allowedElements & ElementTypes.Poison) == ElementTypes.Poison)
            {
                SetSpellElement(ElementTypes.Poison);
                return;
            }
            else if ((allowedElements & ElementTypes.Shock) == ElementTypes.Shock)
            {
                SetSpellElement(ElementTypes.Shock);
                return;
            }
            else if ((allowedElements & ElementTypes.Magic) == ElementTypes.Magic)
            {
                SetSpellElement(ElementTypes.Magic);
                return;
            }
        }

        void SetIcon(SpellIcon icon)
        {
            // Fallback to classic index if no valid icon pack key
            if (string.IsNullOrEmpty(icon.key) || !DaggerfallUI.Instance.SpellIconCollection.HasPack(icon.key))
            {
                icon.key = string.Empty;
                icon.index = icon.index % DaggerfallUI.Instance.SpellIconCollection.SpellIconCount;
            }

            // Set icon
            selectedIcon = icon;
            selectIconButton.BackgroundTexture = DaggerfallUI.Instance.SpellIconCollection.GetSpellIcon(selectedIcon);
        }

        List<EffectEntry> GetEffectEntries()
        {
            // Get a list of actual effect entries and ignore empty slots
            List<EffectEntry> effects = new List<EffectEntry>();
            for (int i = 0; i < maxEffectsPerSpell; i++)
            {
                if (!string.IsNullOrEmpty(effectEntries[i].Key))
                    effects.Add(effectEntries[i]);
            }

            return effects;
        }

        void UpdateSpellCosts()
        {
            // Note: Daggerfall shows gold cost 0 and spellpoint cost 5 with no effects added
            // Not copying this behaviour at this time intentionally as it seems unclear for an invalid
            // spell to have any casting cost at all - may change later
            totalGoldCost = 0;
            totalSpellPointCost = 0;

            // Do nothing when effect editor not setup or not used effect slots
            // This means there is nothing to calculate
            if (effectEditor == null || !effectEditor.IsSetup || CountUsedEffectSlots() == 0)
            {
                SetStatusLabels();
                return;
            }

            // Update slot being edited with current effect editor settings
            if (editOrDeleteSlot != -1)
                effectEntries[editOrDeleteSlot] = effectEditor.EffectEntry;

            // Get total costs
            FormulaHelper.CalculateTotalEffectCosts(effectEntries, selectedTarget, out totalGoldCost, out totalSpellPointCost);
            SetStatusLabels();
        }

        #endregion

        #region Button Events

        private void AddEffectButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            const int noMoreThan3Effects = 1707;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            // Must have a free effect slot
            if (GetFirstFreeEffectSlotIndex() == -1)
            {
                DaggerfallMessageBox mb = new DaggerfallMessageBox(
                    uiManager,
                    DaggerfallMessageBox.CommonMessageBoxButtons.Nothing,
                    DaggerfallUnity.Instance.TextProvider.GetRSCTokens(noMoreThan3Effects),
                    this);
                mb.ClickAnywhereToClose = true;
                mb.Show();
                return;
            }

            // Clear existing
            effectGroupPicker.ListBox.ClearItems();
            tipLabel.Text = string.Empty;

            // TODO: Filter out effects incompatible with any effects already added (e.g. incompatible target types)

            // Populate group names
            string[] groupNames = GameManager.Instance.EntityEffectBroker.GetGroupNames(true, thisMagicStation);
            effectGroupPicker.ListBox.AddItems(groupNames);
            effectGroupPicker.ListBox.SelectedIndex = 0;

            // Show effect group picker
            uiManager.PushWindow(effectGroupPicker);
        }

        private void BuyButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            const int notEnoughGold = 1702;
            const int noSpellBook = 1703;
            const int youMustChooseAName = 1704;
            const int spellHasBeenInscribed = 1705;

            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);

            // Presence of spellbook is also checked earlier
            if (!GameManager.Instance.PlayerEntity.Items.Contains(Items.ItemGroups.MiscItems, (int)Items.MiscItems.Spellbook))
            {
                DaggerfallUI.MessageBox(noSpellBook);
                return;
            }

            // Spell must have at least one effect - adding custom message
            List<EffectEntry> effects = GetEffectEntries();
            if (effects.Count == 0)
            {
                DaggerfallUI.MessageBox(TextManager.Instance.GetText(textDatabase, "noEffectsError"));
                return;
            }

            // Enough money?
            var moneyAvailable = GameManager.Instance.PlayerEntity.GetGoldAmount();
            if (moneyAvailable < totalGoldCost)
            {
                DaggerfallUI.MessageBox(notEnoughGold);
                return;
            }

            // Spell must have a name; Only bother the player if everything else is correct
            if (string.IsNullOrEmpty(spellNameLabel.Text))
            {
                DaggerfallUI.MessageBox(youMustChooseAName);
                return;
            }

            GameManager.Instance.PlayerEntity.DeductGoldAmount(totalGoldCost);

            // Create effect bundle settings
            EffectBundleSettings spell = new EffectBundleSettings();
            spell.Version = EntityEffectBroker.CurrentSpellVersion;
            spell.BundleType = BundleTypes.Spell;
            spell.TargetType = selectedTarget;
            spell.ElementType = selectedElement;
            spell.Name = spellNameLabel.Text;
            spell.IconIndex = selectedIcon.index;
            spell.Icon = selectedIcon;
            spell.Effects = effects.ToArray();

            // Add to player entity spellbook
            GameManager.Instance.PlayerEntity.AddSpell(spell);

            DaggerfallUI.Instance.PlayOneShot(inscribeGrimoire);

            // Notify player and exit when this messagebox is dismissed
            DaggerfallMessageBox mbComplete = DaggerfallUI.MessageBox(spellHasBeenInscribed);
            mbComplete.ClickAnywhereToClose = true;
            mbComplete.PreviousWindow = this;
            mbComplete.OnClose += SpellHasBeenInscribed_OnClose;
            mbComplete.Show();
        }

        private void SpellHasBeenInscribed_OnClose()
        {
            SetDefaults();
            iconPicker.ResetScrollPosition();
        }

        private void NewSpellButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetDefaults();
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void CasterOnlyButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellTarget(TargetTypes.CasterOnly);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void ByTouchButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellTarget(TargetTypes.ByTouch);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void SingleTargetAtRangeButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellTarget(TargetTypes.SingleTargetAtRange);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void AreaAroundCasterButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellTarget(TargetTypes.AreaAroundCaster);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void AreaAtRangeButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellTarget(TargetTypes.AreaAtRange);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void FireBasedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellElement(ElementTypes.Fire);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void ColdBasedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellElement(ElementTypes.Cold);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void PoisonBasedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellElement(ElementTypes.Poison);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void ShockBasedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellElement(ElementTypes.Shock);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void MagicBasedButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            SetSpellElement(ElementTypes.Magic);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void NextIconButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            int index = selectedIcon.index + 1;
            if (index >= DaggerfallUI.Instance.SpellIconCollection.GetIconCount(selectedIcon.key))
                index = 0;

            selectedIcon.index = index;
            SetIcon(selectedIcon);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void SelectIconButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            uiManager.PushWindow(iconPicker);
        }

        private void PreviousIconButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            int index = selectedIcon.index - 1;
            if (index < 0)
                index = DaggerfallUI.Instance.SpellIconCollection.GetIconCount(selectedIcon.key) - 1;

            selectedIcon.index = index;
            SetIcon(selectedIcon);
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
        }

        private void NameSpellButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            DaggerfallInputMessageBox mb = new DaggerfallInputMessageBox(uiManager, this);
            mb.TextBox.Text = spellNameLabel.Text;
            mb.SetTextBoxLabel(TextManager.Instance.GetText("SpellmakerUI", "enterSpellName") + " ");
            mb.OnGotUserInput += EnterName_OnGotUserInput;
            mb.Show();
        }

        private void AddEffectGroupListBox_OnUseSelectedItem()
        {
            // Clear existing
            effectSubGroupPicker.ListBox.ClearItems();
            enumeratedEffectTemplates.Clear();

            // Enumerate subgroup effect key name pairs
            enumeratedEffectTemplates = GameManager.Instance.EntityEffectBroker.GetEffectTemplates(effectGroupPicker.ListBox.SelectedItem, thisMagicStation);
            if (enumeratedEffectTemplates.Count < 1)
                throw new Exception(string.Format("Could not find any effect templates for group {0}", effectGroupPicker.ListBox.SelectedItem));

            // If this is a solo effect without any subgroups names defined (e.g. "Regenerate") then go straight to effect editor
            if (enumeratedEffectTemplates.Count == 1 && string.IsNullOrEmpty(enumeratedEffectTemplates[0].Properties.SubGroupName))
            {
                effectGroupPicker.CloseWindow();
                AddAndEditSlot(enumeratedEffectTemplates[0]);
                //uiManager.PushWindow(effectEditor);
                return;
            }

            // Sort list by subgroup name
            enumeratedEffectTemplates.Sort((s1, s2) => s1.Properties.SubGroupName.CompareTo(s2.Properties.SubGroupName));

            // Populate subgroup names in list box
            foreach (IEntityEffect effect in enumeratedEffectTemplates)
            {
                effectSubGroupPicker.ListBox.AddItem(effect.Properties.SubGroupName);
            }
            effectSubGroupPicker.ListBox.SelectedIndex = 0;

            // Show effect subgroup picker
            // Note: In classic the group name is now shown (and mostly obscured) behind the picker at first available effect slot
            // This is not easily visible and not sure if this really communicates anything useful to user
            // Daggerfall Unity also allows user to cancel via escape back to previous dialog, so changing this beheaviour intentionally
            uiManager.PushWindow(effectSubGroupPicker);
        }

        private void AddEffectSubGroup_OnUseSelectedItem()
        {
            // Close effect pickers
            effectGroupPicker.CloseWindow();
            effectSubGroupPicker.CloseWindow();

            // Get selected effect from those on offer
            IEntityEffect effectTemplate = enumeratedEffectTemplates[effectSubGroupPicker.ListBox.SelectedIndex];
            if (effectTemplate != null)
            {
                AddAndEditSlot(effectTemplate);
                //Debug.LogFormat("Selected effect {0} {1} with key {2}", effectTemplate.GroupName, effectTemplate.SubGroupName, effectTemplate.Key);
            }
        }

        private void EditOrDeleteSpell_OnCancel(DaggerfallPopupWindow sender)
        {
            editOrDeleteSlot = -1;
        }

        private void EditOrDeleteSpell_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            sender.CloseWindow();
        }

        private void DeleteButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Delete effect entry
            DaggerfallUI.Instance.PlayOneShot(SoundClips.ButtonClick);
            ClearPendingDeleteEffectSlot();
            UpdateSpellCosts();
        }

        private void EditButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            // Edit effect entry
            effectEditor.EffectEntry = effectEntries[editOrDeleteSlot];
            uiManager.PushWindow(effectEditor);
        }

        #endregion

        #region Effect Editor Events

        private void EffectEditor_OnClose()
        {
            editOrDeleteSlot = -1;
            UpdateAllowedButtons();
        }

        private void EffectEditor_OnSettingsChanged()
        {
            UpdateSpellCosts();
        }

        private void IconPicker_OnClose()
        {
            if (iconPicker.SelectedIcon != null)
                SetIcon(iconPicker.SelectedIcon.Value);
        }

        private void Effect1NamePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EditOrDeleteSlot(0);
        }

        private void Effect2NamePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EditOrDeleteSlot(1);
        }

        private void Effect3NamePanel_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            EditOrDeleteSlot(2);
        }

        private void EnterName_OnGotUserInput(DaggerfallInputMessageBox sender, string input)
        {
            spellNameLabel.Text = input;
        }

        #endregion

        #region Tip Events

        bool lockTip = false;
        private void TipButton_OnMouseEnter(BaseScreenComponent sender)
        {
            // Lock tip if already has text, this means we are changing directly to adjacent button
            // This prevents OnMouseLeave event from previous button wiping tip text of new button
            if (!string.IsNullOrEmpty(tipLabel.Text))
                lockTip = true;

            tipLabel.Text = TextManager.Instance.GetText(textDatabase, sender.Tag as string);
            if (sender is Button)
            {
                Button buttonSender = (Button)sender;
                if (buttonSender.Hotkey != HotkeySequence.None)
                    tipLabel.Text += string.Format(" ({0})", buttonSender.Hotkey);
            }
        }

        private void TipButton_OnMouseLeave(BaseScreenComponent sender)
        {
            // Clear tip when not locked, otherwise reset tip lock
            if (!lockTip)
                tipLabel.Text = string.Empty;
            else
                lockTip = false;
        }

        #endregion
    }
}