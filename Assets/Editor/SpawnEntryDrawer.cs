﻿using System.Collections;
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
        int height = 102;
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

        position.width -= 4;
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        bool selected = property.FindPropertyRelative("Previewing").boolValue;
        string error = string.Empty;
        string warning = string.Empty;
        int type = property.FindPropertyRelative("Type").enumValueIndex;
        GameObject obj;
        SerializedProperty specificsExpanded = property.FindPropertyRelative("SpecificsExpanded");

        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(),
            padding = new RectOffset(),
            fontStyle = FontStyle.Bold
        };

        GUIStyle smallLabelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(),
            padding = new RectOffset(),
            fontStyle = FontStyle.Normal
        };

        //General Rects
        var bgRect = new Rect(position.x - 4, position.y, position.width + 8, position.height);
        position.y += 4;
        obj = (GameObject)property.FindPropertyRelative("Entity").objectReferenceValue;
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
        var typeSpecificDropdownRect = new Rect(position.x, position.y, 25, 18);

        //Drawing everything
        if (selected) EditorGUI.DrawRect(bgRect, new Color(38 / 255f, 127 / 255f, 0f));
        else EditorGUI.DrawRect(bgRect, Color.gray);
        if(warning != string.Empty)
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
        EditorGUI.DrawRect(specificsHeaderRect, new Color(91 / 255f, 91 / 255f, 91 / 255f));
        if (GUI.Button(typeSpecificDropdownRect, new GUIContent(specificsExpanded.boolValue ? "▲" : "▼")))
            specificsExpanded.boolValue = !specificsExpanded.boolValue;
        if (type == 0) //Enemy Specific
        {
            EditorGUI.PropertyField(speedRect, property.FindPropertyRelative("SpeedConst"), new GUIContent("Speed"));
            EditorGUI.LabelField(typeSpecificRect, "Enemy Specific", labelStyle);
            if (specificsExpanded.boolValue)
            {
                position.y += 18;
                var specificsBGRect = new Rect(position.x, position.y, position.width, 18);
                EditorGUI.DrawRect(specificsBGRect, new Color(91 / 255f, 91 / 255f, 91 / 255f));
                EditorGUIUtility.labelWidth = 35.0f;
                var pathRect = new Rect(position.x, position.y, position.width - 43, 18);
                EditorGUI.PropertyField(pathRect, property.FindPropertyRelative("Path"), new GUIContent("Path"));
                EditorGUIUtility.labelWidth = 25.0f;
                var flipPathRect = new Rect(position.x + position.width - 43, position.y, position.width - 43, 18);
                EditorGUI.PropertyField(flipPathRect, property.FindPropertyRelative("FlipPath"), new GUIContent("Flip"));
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
                EditorGUI.DrawRect(specificsBGRect, new Color(91 / 255f, 91 / 255f, 91 / 255f));

                var offscrBhvRect = new Rect(position.x, position.y, position.width, 18);
                if (property.FindPropertyRelative("OffScrBhv").enumValueIndex == 1) offscrBhvRect.width -= 35;
                var bouncesRect = new Rect(position.x + position.width - 35, position.y, 35, 18);
                EditorGUIUtility.labelWidth = 10.0f;
                EditorGUI.PropertyField(bouncesRect, property.FindPropertyRelative("Bounces"), new GUIContent("X"));
                EditorGUIUtility.labelWidth = 60.0f;
                EditorGUI.PropertyField(offscrBhvRect, property.FindPropertyRelative("OffScrBhv"), new GUIContent("OffscrBhv", "Offscreen Behaviour"));

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
                            EditorGUI.DrawRect(ringBGRect, new Color(91 / 255f, 91 / 255f, 91 / 255f));
                            var bulletRect = new Rect(position.x, position.y, position.width - 35, 18);
                            var elementsRect = new Rect(position.x + position.width - 35, position.y, 35, 18);
                            EditorGUIUtility.labelWidth = 10.0f;
                            EditorGUI.PropertyField(elementsRect, property.FindPropertyRelative("RingElements"), new GUIContent("X"));
                            EditorGUIUtility.labelWidth = 40.0f;
                            EditorGUI.PropertyField(bulletRect, property.FindPropertyRelative("RingBullets"), new GUIContent("Bullets"));
                            position.y += 18;
                            var baseDistanceRect = new Rect(position.x, position.y, position.width, 18);
                            EditorGUIUtility.labelWidth = 85.0f;
                            EditorGUI.PropertyField(baseDistanceRect, property.FindPropertyRelative("RingDistance"), new GUIContent("Base Distance"));
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

        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}