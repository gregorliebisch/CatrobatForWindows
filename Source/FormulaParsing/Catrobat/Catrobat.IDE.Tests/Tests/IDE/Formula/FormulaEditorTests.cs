﻿using System;
using System.Collections.Generic;
using System.Linq;
using Catrobat.IDE.Core.CatrobatObjects.Formulas.FormulaToken;
using Catrobat.IDE.Core.CatrobatObjects.Variables;
using Catrobat.IDE.Core.ExtensionMethods;
using Catrobat.IDE.Core.FormulaEditor.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Catrobat.IDE.Tests.Tests.IDE.Formula
{
    [TestClass]
    public class FormulaEditorTests
    {

        [TestMethod]
        public void TestConstants()
        {
            TestKey(FormulaEditorKey.D0, FormulaTokenFactory.CreateNumberToken(0));
            TestKey(FormulaEditorKey.D1, FormulaTokenFactory.CreateNumberToken(1));
            TestKey(FormulaEditorKey.D2, FormulaTokenFactory.CreateNumberToken(2));
            TestKey(FormulaEditorKey.D3, FormulaTokenFactory.CreateNumberToken(3));
            TestKey(FormulaEditorKey.D4, FormulaTokenFactory.CreateNumberToken(4));
            TestKey(FormulaEditorKey.D5, FormulaTokenFactory.CreateNumberToken(5));
            TestKey(FormulaEditorKey.D6, FormulaTokenFactory.CreateNumberToken(6));
            TestKey(FormulaEditorKey.D7, FormulaTokenFactory.CreateNumberToken(7));
            TestKey(FormulaEditorKey.D8, FormulaTokenFactory.CreateNumberToken(8));
            TestKey(FormulaEditorKey.D9, FormulaTokenFactory.CreateNumberToken(9));
            TestKey(FormulaEditorKey.DecimalSeparator, FormulaTokenFactory.CreateDecimalSeparatorToken());
            TestKey(FormulaEditorKey.Pi, FormulaTokenFactory.CreatePiToken());
            TestKey(FormulaEditorKey.True, FormulaTokenFactory.CreateTrueToken());
            TestKey(FormulaEditorKey.False, FormulaTokenFactory.CreateFalseToken());
        }


        [TestMethod]
        public void TestOperators()
        {
            TestKey(FormulaEditorKey.Plus, FormulaTokenFactory.CreatePlusToken());
            TestKey(FormulaEditorKey.Minus, FormulaTokenFactory.CreateMinusToken());
            TestKey(FormulaEditorKey.Multiply, FormulaTokenFactory.CreateMultiplyToken());
            TestKey(FormulaEditorKey.Divide, FormulaTokenFactory.CreateDivideToken());
            TestKey(FormulaEditorKey.Caret, FormulaTokenFactory.CreateCaretToken());
            TestKey(FormulaEditorKey.Equals, FormulaTokenFactory.CreateEqualsToken());
            TestKey(FormulaEditorKey.NotEquals, FormulaTokenFactory.CreateNotEqualsToken());
            TestKey(FormulaEditorKey.Less, FormulaTokenFactory.CreateLessToken());
            TestKey(FormulaEditorKey.LessEqual, FormulaTokenFactory.CreateLessEqualToken());
            TestKey(FormulaEditorKey.Greater, FormulaTokenFactory.CreateGreaterToken());
            TestKey(FormulaEditorKey.GreaterEqual, FormulaTokenFactory.CreateGreaterEqualToken());
            TestKey(FormulaEditorKey.And, FormulaTokenFactory.CreateAndToken());
            TestKey(FormulaEditorKey.Or, FormulaTokenFactory.CreateOrToken());
            TestKey(FormulaEditorKey.Not, FormulaTokenFactory.CreateNotToken());
            TestKey(FormulaEditorKey.Mod, FormulaTokenFactory.CreateModToken());
        }

        [TestMethod]
        public void TestFunctions()
        {
            TestKey(FormulaEditorKey.Exp, new IFormulaToken[] { FormulaTokenFactory.CreateExpToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Log, new IFormulaToken[] { FormulaTokenFactory.CreateLogToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Ln, new IFormulaToken[] { FormulaTokenFactory.CreateLnToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Min, new IFormulaToken[] {  FormulaTokenFactory.CreateMinToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Max, new IFormulaToken[] { FormulaTokenFactory.CreateMaxToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Sin, new IFormulaToken[] { FormulaTokenFactory.CreateSinToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Cos, new IFormulaToken[] { FormulaTokenFactory.CreateCosToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Tan, new IFormulaToken[] { FormulaTokenFactory.CreateTanToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Arcsin, new IFormulaToken[] { FormulaTokenFactory.CreateArcsinToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Arccos, new IFormulaToken[] { FormulaTokenFactory.CreateArccosToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Arctan, new IFormulaToken[] { FormulaTokenFactory.CreateArctanToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Sqrt, new IFormulaToken[] { FormulaTokenFactory.CreateSqrtToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Abs, new IFormulaToken[] { FormulaTokenFactory.CreateAbsToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Round, new IFormulaToken[] { FormulaTokenFactory.CreateRoundToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
            TestKey(FormulaEditorKey.Random, new IFormulaToken[] { FormulaTokenFactory.CreateRandomToken(), FormulaTokenFactory.CreateParenthesisToken(true) });
        }

        [TestMethod]
        public void TestSensors()
        {
            TestKey(FormulaEditorKey.AccelerationX, FormulaTokenFactory.CreateAccelerationXToken());
            TestKey(FormulaEditorKey.AccelerationY, FormulaTokenFactory.CreateAccelerationYToken());
            TestKey(FormulaEditorKey.AccelerationZ, FormulaTokenFactory.CreateAccelerationZToken());
            TestKey(FormulaEditorKey.Compass, FormulaTokenFactory.CreateCompassToken());
            TestKey(FormulaEditorKey.InclinationX, FormulaTokenFactory.CreateInclinationXToken());
            TestKey(FormulaEditorKey.InclinationY, FormulaTokenFactory.CreateInclinationYToken());
            TestKey(FormulaEditorKey.Loudness, FormulaTokenFactory.CreateLoudnessToken());
        }

        [TestMethod]
        public void TestProperties()
        {
            TestKey(FormulaEditorKey.Brightness, FormulaTokenFactory.CreateBrightnessToken());
            TestKey(FormulaEditorKey.Layer, FormulaTokenFactory.CreateLayerToken());
            TestKey(FormulaEditorKey.Transparency, FormulaTokenFactory.CreateTransparencyToken());
            TestKey(FormulaEditorKey.PositionX, FormulaTokenFactory.CreatePositionXToken());
            TestKey(FormulaEditorKey.PositionY, FormulaTokenFactory.CreatePositionYToken());
            TestKey(FormulaEditorKey.Rotation, FormulaTokenFactory.CreateRotationToken());
            TestKey(FormulaEditorKey.Size, FormulaTokenFactory.CreateSizeToken());
        }

        [TestMethod]
        public void TestVariables()
        {
            var testVariable = new UserVariable { Name = "TestVariable" };
            TestKey(FormulaEditorKey.LocalVariable, testVariable, FormulaTokenFactory.CreateLocalVariableToken(testVariable));
            TestKey(FormulaEditorKey.GlobalVariable, testVariable, FormulaTokenFactory.CreateGlobalVariableToken(testVariable));
        }

        [TestMethod]
        public void TestBrackets()
        {
            TestKey(FormulaEditorKey.OpeningParenthesis, FormulaTokenFactory.CreateParenthesisToken(true));
            TestKey(FormulaEditorKey.ClosingParenthesis, FormulaTokenFactory.CreateParenthesisToken(false));
        }

        [TestMethod]
        public void TestDelete()
        {
            var editor = new FormulaEditor3();
            Assert.IsFalse(editor.HandleKey(FormulaEditorKey.Delete, null));
            Assert.IsTrue(editor.HandleKey(FormulaEditorKey.Exp, null));
            Assert.IsTrue(editor.HandleKey(FormulaEditorKey.Delete, null));
            Assert.IsTrue(editor.HandleKey(FormulaEditorKey.Delete, null));
            Assert.IsFalse(editor.HandleKey(FormulaEditorKey.Delete, null));
            Assert.IsTrue(editor.HandleKey(FormulaEditorKey.Exp, null));
        }

        [TestMethod]
        public void TestUndoRedo()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void MonkeyTest()
        {
            const int iterations = 1000;
            const int pressedKeys = 10;
            var random = new Random();
            var keys = EnumExtensions.AsEnumerable<FormulaEditorKey>().ToList();
            for (var iteration = 1; iteration <= iterations; iteration++)
            {
                var editor = new FormulaEditor3();
                for (var i = 1; i <= pressedKeys; i++)
                {
                    var randomKey = keys.ElementAt(random.Next(0, keys.Count));
                    editor.HandleKey(randomKey, null);
                }
            }
        }

        #region Helpers

        private void TestKey(FormulaEditorKey key, IFormulaToken expectedToken)
        {
            var editor = new FormulaEditor3();
            Assert.IsTrue(editor.HandleKey(key, null));
            Assert.AreEqual(expectedToken, editor.Tokens.Single());
        }

        private void TestKey(FormulaEditorKey key, IEnumerable<IFormulaToken> expectedTokens)
        {
            var editor = new FormulaEditor3();
            Assert.IsTrue(editor.HandleKey(key, null));
            EnumerableAssert.AreEqual(expectedTokens, editor.Tokens);
        }

        private void TestKey(FormulaEditorKey key, UserVariable variable, IFormulaToken expectedToken)
        {
            var editor = new FormulaEditor3();
            Assert.IsTrue(editor.HandleKey(key, variable));
            Assert.AreEqual(expectedToken, editor.Tokens.Single());
        }

        #endregion
    }
}
