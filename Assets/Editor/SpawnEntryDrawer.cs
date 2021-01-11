using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Bullets;
using System.Reflection;

[CustomPropertyDrawer(typeof(SpawnEntry))]
public class SpawnEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int height = 120;
        if (!property.FindPropertyRelative("Expanded").boolValue)
        {
            height = 25;
            if (property.FindPropertyRelative("hasErrors").boolValue) height += 36;
            if (property.FindPropertyRelative("hasWarnings").boolValue) height += 36;
            return height;
        }
        if(property.FindPropertyRelative("SpecificsExpanded").boolValue)
        {
            if (property.FindPropertyRelative("Type").enumValueIndex == 0)
            {
                height += 36;
            }
            else if (property.FindPropertyRelative("Type").enumValueIndex == 1)
            {
                height += 54;

                GameObject obj = (GameObject)property.FindPropertyRelative("Entity").objectReferenceValue;
                obj.TryGetComponent<BulletBase>(out BulletBase bullet);
                if (bullet)
                {
                    switch (bullet.Type)
                    {
                        case BulletType.Ring:
                            height += 36;
                            break;
                    }
                }
            }
        }
        else height += 18;
        if (property.FindPropertyRelative("hasErrors").boolValue) height += 36;
        if (property.FindPropertyRelative("hasWarnings").boolValue) height += 36;

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position.width -= 6;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        bool selected = property.FindPropertyRelative("Previewing").boolValue;
        string error = string.Empty;
        string warning = string.Empty;
        int type = property.FindPropertyRelative("Type").enumValueIndex;
        GameObject obj;
        SerializedProperty specificsExpanded = property.FindPropertyRelative("SpecificsExpanded");
        SerializedProperty expanded = property.FindPropertyRelative("Expanded");
        SerializedProperty pendingAction = property.FindPropertyRelative("PendingAction");

        EditorStyles.label.richText = true;

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(),
            padding = new RectOffset(),
            fontStyle = FontStyle.Bold,
            richText = true
        };
        labelStyle.normal.textColor = Color.white;

        GUIStyle smallLabelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(),
            padding = new RectOffset(),
            fontStyle = FontStyle.Normal,
            richText = true
        };
        smallLabelStyle.normal.textColor = Color.white;

        Color BGColorInactive = Color.gray;
        Color BGColorActive = new Color(38 / 255f, 127 / 255f, 0f);
        Color LabelColorInactive = new Color(91 / 255f, 91 / 255f, 91 / 255f);
        Color LabelColorActive = new Color(29 / 255f, 99 / 255f, 0f);

        //General Rects
        var bgRect = new Rect(position.x - 4, position.y, position.width + 8, position.height);
        position.y += 4;
        obj = (GameObject)property.FindPropertyRelative("Entity").objectReferenceValue;
        if (obj)
        {
            if (type == 0)
            {
                if (property.FindPropertyRelative("Path").objectReferenceValue == null) error = "This Enemy doesn't have a Path!";
                if (!obj.GetComponent<EnemyBase>()) error = "Entity isn't an Enemy!";
                if (property.FindPropertyRelative("SpeedConst").floatValue == 0) warning = "Speed is 0!";
            }
            if (property.FindPropertyRelative("Entity").objectReferenceValue == null) error = "You have to declare an entity to spawn!";
            if (type == 1)
            {
                if (!obj.GetComponent<BulletBase>()) error = "Entity isn't a Bullet!";
                if (property.FindPropertyRelative("RingBullets").objectReferenceValue == null &&
                    obj.GetComponent<BulletBase>().Type == BulletType.Ring) error = "This Ring doesn't have assigned Child Bullets!";
                Vector2 speedRange = property.FindPropertyRelative("SpeedRange").vector2Value;
                if (speedRange.x == 0 || speedRange.y == 0) warning = "Speed can be 0!";
            }
        }
        else error = "No Entity Selected!";

        var headerRect = new Rect(position.x + 18, position.y, position.width - 90, 18);
        var infoDropdownRect = new Rect(position.x, position.y, 18, 18);
        var moveDownButton = new Rect(position.x + position.width - 72, position.y, 18, 18);
        var moveUpButton = new Rect(position.x + position.width - 54, position.y, 18, 18);
        var deleteButton = new Rect(position.x + position.width - 36, position.y, 18, 18);
        var cloneButton = new Rect(position.x + position.width - 18, position.y, 18, 18);
        position.y += 18;
        var warnRect = new Rect(position.x, position.y, position.width, 32);
        if(error != string.Empty || warning != string.Empty) position.y += 36;
        var typeRect = new Rect(position.x, position.y, position.width, 18);
        position.y += 18;
        var entityRect = new Rect(position.x, position.y, position.width - 35, 18);
        var delayRect = new Rect(position.x + position.width - 35, position.y, 35, 18);
        position.y += 18;
        var positionRect = new Rect(position.x, position.y, position.width, 18);
        var rotationRect = new Rect(position.x + position.width - 75, position.y, 75, 18);
        position.y += 18;
        var speedRect = new Rect(position.x, position.y, position.width, 18);
        position.y += 18;
        var typeSpecificRect = new Rect(position.x, position.y, position.width, 18);
        var typeSpecificDropdownRect = new Rect(position.x, position.y, 18, 18);

        //Header
        if (selected) EditorGUI.DrawRect(bgRect, BGColorActive);
        else EditorGUI.DrawRect(bgRect, BGColorInactive);
        if (selected) EditorGUI.DrawRect(headerRect, LabelColorActive);
        else EditorGUI.DrawRect(headerRect, LabelColorInactive);
        if(obj)
            EditorGUI.LabelField(headerRect, $"{property.FindPropertyRelative("Entity").objectReferenceValue.name} - " +
                $"{property.FindPropertyRelative("SpawnTime").floatValue}s", labelStyle);
        else
            EditorGUI.LabelField(headerRect, $"<color=red>No Entity</color> - Spawntime: " +
                $"{property.FindPropertyRelative("SpawnTime").floatValue}s", labelStyle);
        if (GUI.Button(infoDropdownRect, new GUIContent(expanded.boolValue ? "Λ" : "V"), EditorStyles.miniButtonRight))
            expanded.boolValue = !expanded.boolValue;
        //Drawing List utility Buttons
        if (GUI.Button(cloneButton, new GUIContent("+"), EditorStyles.miniButtonRight))
            pendingAction.enumValueIndex = (int)SpawnEntry.Actions.Clone;
        if (GUI.Button(deleteButton, new GUIContent("X"), EditorStyles.miniButtonMid))
            pendingAction.enumValueIndex = (int)SpawnEntry.Actions.Delete;
        if (GUI.Button(moveUpButton, new GUIContent("\u2191"), EditorStyles.miniButtonMid))
            pendingAction.enumValueIndex = (int)SpawnEntry.Actions.MoveUp;
        if (GUI.Button(moveDownButton, new GUIContent("\u2193"), EditorStyles.miniButtonLeft))
            pendingAction.enumValueIndex = (int)SpawnEntry.Actions.MoveDown;
        //Drawing errors and warnings
        if (warning != string.Empty)
        {
            EditorGUI.DrawRect(warnRect, new Color(231 / 255f, 179 / 255f, 43 / 255f));
            EditorGUI.HelpBox(warnRect, warning, MessageType.Warning);
            property.FindPropertyRelative("hasWarnings").boolValue = true;
        }
        else property.FindPropertyRelative("hasWarnings").boolValue = false;
        if (error != string.Empty)
        {
            EditorGUI.DrawRect(warnRect, new Color(91 / 255f, 91 / 255f, 91 / 255f));
            EditorGUI.HelpBox(warnRect, error, MessageType.Error);
            property.FindPropertyRelative("hasErrors").boolValue = true;
        }
        else property.FindPropertyRelative("hasErrors").boolValue = false;
        //Drawing everything else
        if (expanded.boolValue)
        {
            EditorGUIUtility.labelWidth = 65.0f;
            EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("Type"), new GUIContent("Entry Type"));
            EditorGUIUtility.labelWidth = 35.0f;
            EditorGUI.PropertyField(entityRect, property.FindPropertyRelative("Entity"), new GUIContent("Entity"));
            EditorGUI.PropertyField(delayRect, property.FindPropertyRelative("Delay"), GUIContent.none);
            EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("Position"), new GUIContent("Pos", "Position"));
            EditorGUIUtility.labelWidth = 25.0f;
            EditorGUI.PropertyField(rotationRect, property.FindPropertyRelative("Rotation"), new GUIContent("Rot", "Z Rotation"));
            EditorGUIUtility.labelWidth = 40.0f;
            var specificsHeaderRect = new Rect(position.x, position.y, position.width, 18);
            if (selected) EditorGUI.DrawRect(specificsHeaderRect, LabelColorActive);
            else EditorGUI.DrawRect(specificsHeaderRect, LabelColorInactive);
            if (GUI.Button(typeSpecificDropdownRect, new GUIContent(specificsExpanded.boolValue ? "Λ" : "V", "Expand Entry")))
                specificsExpanded.boolValue = !specificsExpanded.boolValue;

            if (type == 0) //Enemy Specific
            {
                EditorGUI.PropertyField(speedRect, property.FindPropertyRelative("SpeedConst"), new GUIContent("Speed"));
                EditorGUI.LabelField(typeSpecificRect, "Enemy Specific", labelStyle);
                if (specificsExpanded.boolValue)
                {
                    position.y += 18;
                    var specificsBGRect = new Rect(position.x, position.y, position.width, 18);
                    if (selected) EditorGUI.DrawRect(specificsBGRect, LabelColorActive);
                    else EditorGUI.DrawRect(specificsBGRect, LabelColorInactive);
                    EditorGUIUtility.labelWidth = 35.0f;
                    var pathRect = new Rect(position.x, position.y, position.width - 43, 18);
                    EditorGUI.PropertyField(pathRect, property.FindPropertyRelative("Path"), new GUIContent("<color=white>" + "Path" + "</color>"));
                    EditorGUIUtility.labelWidth = 25.0f;
                    var flipPathRect = new Rect(position.x + position.width - 43, position.y, position.width - 43, 18);
                    EditorGUI.PropertyField(flipPathRect, property.FindPropertyRelative("FlipPath"), new GUIContent("<color=white>" + "Flip" + "</color>"));
                }
            }
            if (type == 1) //Bullet Specific
            {
                EditorGUI.PropertyField(speedRect, property.FindPropertyRelative("SpeedRange"), new GUIContent("Speed"));
                EditorGUI.LabelField(typeSpecificRect, "Bullet Specific", labelStyle);
                if (specificsExpanded.boolValue)
                {
                    position.y += 18;
                    var specificsBGRect = new Rect(position.x, position.y, position.width, 36);
                    if (selected) EditorGUI.DrawRect(specificsBGRect, LabelColorActive);
                    else EditorGUI.DrawRect(specificsBGRect, LabelColorInactive);

                    var offscrBhvRect = new Rect(position.x, position.y, position.width, 18);
                    if (property.FindPropertyRelative("OffScrBhv").enumValueIndex == 1) offscrBhvRect.width -= 35;
                    var bouncesRect = new Rect(position.x + position.width - 35, position.y, 35, 18);
                    EditorGUIUtility.labelWidth = 10.0f;
                    EditorGUI.PropertyField(bouncesRect, property.FindPropertyRelative("Bounces"), new GUIContent("<color=white>" + "X" + "</color>"));
                    EditorGUIUtility.labelWidth = 60.0f;
                    EditorGUI.PropertyField(offscrBhvRect, property.FindPropertyRelative("OffScrBhv"), new GUIContent("<color=white>" + "OffscrBhv" + "</color>", "Offscreen Behaviour"));

                    obj = (GameObject)property.FindPropertyRelative("Entity").objectReferenceValue;
                    obj.TryGetComponent(out BulletBase bullet);
                    position.y += 18;
                    var bulletTypeRect = new Rect(position.x, position.y, position.width, 18);
                    if (bullet) switch (bullet.Type)
                        {
                            case BulletType.Petal:
                            case BulletType.Base:
                                EditorGUI.LabelField(bulletTypeRect, "This Bullet doesn't have unique fields", smallLabelStyle);
                                break;
                            case BulletType.Ring:
                                EditorGUI.LabelField(bulletTypeRect, "Unique Ring Bullet fields", smallLabelStyle);
                                position.y += 18;
                                var ringBGRect = new Rect(position.x, position.y, position.width, 38);
                                if (selected) EditorGUI.DrawRect(ringBGRect, LabelColorActive);
                                else EditorGUI.DrawRect(ringBGRect, LabelColorInactive);
                                var bulletRect = new Rect(position.x, position.y, position.width - 35, 18);
                                var elementsRect = new Rect(position.x + position.width - 35, position.y, 35, 18);
                                EditorGUIUtility.labelWidth = 10.0f;
                                EditorGUI.PropertyField(elementsRect, property.FindPropertyRelative("RingElements"), new GUIContent("<color=white>" + "X" + "</color>"));
                                EditorGUIUtility.labelWidth = 40.0f;
                                EditorGUI.PropertyField(bulletRect, property.FindPropertyRelative("RingBullets"), new GUIContent("<color=white>" + "Bullets" + "</color>"));
                                position.y += 18;
                                var baseDistanceRect = new Rect(position.x, position.y, position.width, 18);
                                EditorGUIUtility.labelWidth = 85.0f;
                                EditorGUI.PropertyField(baseDistanceRect, property.FindPropertyRelative("RingDistance"), new GUIContent("<color=white>" + "Base Distance" + "</color>"));
                                break;
                        }
                }
            }

            //Preview Button
            position.y += 22;
            var previewButtonRect = new Rect(position.x, position.y, position.width, 18);
            if (GUI.Button(previewButtonRect, selected ? "Stop Previewing" : "Preview"))
            {
                property.FindPropertyRelative("Previewing").boolValue = !selected;
            }
        }
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}