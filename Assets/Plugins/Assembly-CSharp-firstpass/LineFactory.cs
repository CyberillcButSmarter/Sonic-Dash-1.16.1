using System;
using System.Collections.Generic;
using UnityEngine;

public class LineFactory
{
	public enum LineType
	{
		None = 0,
		A = 1,
		E = 2,
		F = 3,
		H = 4,
		I = 5,
		K = 6,
		L = 7,
		M = 8,
		N = 9,
		T = 10,
		V = 11,
		W = 12,
		X = 13,
		Y = 14,
		Z = 15,
		Number1 = 16,
		Number4 = 17,
		Number7 = 18,
		Plus = 19,
		Minus = 20,
		RightCheck = 21,
		LeftCheck = 22,
		Square = 23,
		Rectangle = 24
	}

	private LineSwipeBase[] lineSwipes;

	private static LineFactory _factory;

	public LineFactory()
	{
		lineSwipes = new LineSwipeBase[Enum.GetNames(typeof(LineType)).Length];
		MultipleLineSwipe multipleLineSwipe = new MultipleLineSwipe(LineType.A.ToString());
		lineSwipes[1] = multipleLineSwipe;
		CompoundLineSwipe compoundLineSwipe = new CompoundLineSwipe(LineType.A.ToString());
		LineSwipe lineSwipe = new LineSwipe(LineType.A.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe);
		LineSwipe lineSwipe2 = new LineSwipe(LineType.A.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Right));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, 0.5f));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.A.ToString());
		lineSwipe = new LineSwipe(LineType.A.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.A.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe2);
		LineSwipe lineSwipe3 = new LineSwipe(LineType.A.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Right));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe3, 0, 0.5f));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		multipleLineSwipe = new MultipleLineSwipe(LineType.E.ToString());
		lineSwipes[2] = multipleLineSwipe;
		compoundLineSwipe = new CompoundLineSwipe(LineType.E.ToString());
		lineSwipe = new LineSwipe(LineType.E.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.E.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.E.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		LineSwipe lineSwipe4 = new LineSwipe(LineType.E.ToString());
		lineSwipe4.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe4);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Lower, lineSwipe4, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.E.ToString());
		lineSwipe = new LineSwipe(LineType.E.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.E.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.E.ToString());
		lineSwipe = new LineSwipe(LineType.E.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.E.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.E.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.Lower, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.E.ToString());
		lineSwipe = new LineSwipe(LineType.E.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.E.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.E.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		multipleLineSwipe = new MultipleLineSwipe(LineType.F.ToString());
		lineSwipes[3] = multipleLineSwipe;
		compoundLineSwipe = new CompoundLineSwipe(LineType.F.ToString());
		lineSwipe = new LineSwipe(LineType.F.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.F.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.F.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 0, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.4f, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.F.ToString());
		lineSwipe = new LineSwipe(LineType.F.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.F.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, 0.4f, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.H.ToString());
		lineSwipe = new LineSwipe(LineType.H.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.H.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.H.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe2, 0, 0.5f, lineSwipe3, 0, LineRelationship.LinePosition.Right));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe3, 0, LineRelationship.LinePosition.Left));
		lineSwipes[4] = compoundLineSwipe;
		lineSwipe = new LineSwipe(LineType.I.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		lineSwipes[5] = lineSwipe;
		multipleLineSwipe = new MultipleLineSwipe(LineType.K.ToString());
		lineSwipes[6] = multipleLineSwipe;
		compoundLineSwipe = new CompoundLineSwipe(LineType.K.ToString());
		lineSwipe = new LineSwipe(LineType.K.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.K.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.K.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Smallest));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.6f, lineSwipe2, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe2, 0, 0.1f, lineSwipe3, 0, LineRelationship.LinePosition.Start));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.K.ToString());
		lineSwipe = new LineSwipe(LineType.K.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.K.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.End));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe2, 0, LineRelationship.LinePosition.Left));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		lineSwipe = new LineSwipe(LineType.L.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Smallest));
		lineSwipe.GetSegment(0).matchLengthDiffPercent = 0.5f;
		lineSwipes[7] = lineSwipe;
		lineSwipe = new LineSwipe(LineType.M.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, SwipeGesture.SwipeDirection.Plus45, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, SwipeGesture.SwipeDirection.Down, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, SwipeGesture.SwipeDirection.Up, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, SwipeGesture.SwipeDirection.Plus135, Segment.SizeSpec.Ratio));
		lineSwipes[8] = lineSwipe;
		multipleLineSwipe = new MultipleLineSwipe(LineType.N.ToString());
		lineSwipes[9] = multipleLineSwipe;
		lineSwipe = new LineSwipe(LineType.N.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		lineSwipe.GetSegment(1).matchLengthDiffPercent = 0.6f;
		multipleLineSwipe.AddLineSwipe(lineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.N.ToString());
		lineSwipe = new LineSwipe(LineType.N.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.N.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.Lower, lineSwipe2, 0, LineRelationship.LinePosition.Lower));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.N.ToString());
		lineSwipe = new LineSwipe(LineType.N.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.N.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.N.ToString());
		lineSwipe = new LineSwipe(LineType.N.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.N.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.N.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Upper, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe2, 0, LineRelationship.LinePosition.Lower, lineSwipe3, 0, LineRelationship.LinePosition.Lower));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.T.ToString());
		lineSwipe = new LineSwipe(LineType.T.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.T.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Bigger));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenLeftRight, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		lineSwipes[10] = compoundLineSwipe;
		lineSwipe = new LineSwipe(LineType.V.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Ratio));
		lineSwipes[11] = lineSwipe;
		lineSwipe = new LineSwipe(LineType.W.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, SwipeGesture.SwipeDirection.Down, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, SwipeGesture.SwipeDirection.Up, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, SwipeGesture.SwipeDirection.Down, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, SwipeGesture.SwipeDirection.Up, Segment.SizeSpec.Bigger));
		lineSwipes[12] = lineSwipe;
		compoundLineSwipe = new CompoundLineSwipe(LineType.X.ToString());
		lineSwipe = new LineSwipe(LineType.X.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.X.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe2, 0, 0.5f));
		lineSwipes[13] = compoundLineSwipe;
		multipleLineSwipe = new MultipleLineSwipe(LineType.Y.ToString());
		lineSwipes[14] = multipleLineSwipe;
		compoundLineSwipe = new CompoundLineSwipe(LineType.Y.ToString());
		lineSwipe = new LineSwipe(LineType.Y.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Y.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.End, lineSwipe2, 0, LineRelationship.LinePosition.Start));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.Y.ToString());
		lineSwipe = new LineSwipe(LineType.Y.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Y.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		lineSwipe3 = new LineSwipe(LineType.Y.ToString());
		lineSwipe3.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe3);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Lower, lineSwipe2, 0, LineRelationship.LinePosition.Upper));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.Lower, lineSwipe3, 0, LineRelationship.LinePosition.Upper));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.Y.ToString());
		lineSwipe = new LineSwipe(LineType.Y.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Y.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe2, 0, LineRelationship.LinePosition.Lower));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		lineSwipe = new LineSwipe(LineType.Z.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipes[15] = lineSwipe;
		lineSwipe = new LineSwipe(LineType.Number1.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		lineSwipes[16] = lineSwipe;
		multipleLineSwipe = new MultipleLineSwipe(LineType.Number4.ToString());
		lineSwipes[17] = multipleLineSwipe;
		lineSwipe = new LineSwipe(LineType.Number4.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Bigger));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		multipleLineSwipe.AddLineSwipe(lineSwipe);
		compoundLineSwipe = new CompoundLineSwipe(LineType.Y.ToString());
		lineSwipe = new LineSwipe(LineType.Y.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, SwipeGesture.SwipeDirection.Minus135, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Y.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Biggest));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, 0.7f, lineSwipe2, 0, 0.5f));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 1, LineRelationship.LinePosition.BetweenLeftRight, lineSwipe2, 0, 0.5f));
		multipleLineSwipe.AddLineSwipe(compoundLineSwipe);
		lineSwipe = new LineSwipe(LineType.Number7.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Bigger));
		lineSwipes[18] = lineSwipe;
		compoundLineSwipe = new CompoundLineSwipe(LineType.Plus.ToString());
		lineSwipe = new LineSwipe(LineType.Plus.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe);
		lineSwipe2 = new LineSwipe(LineType.Plus.ToString());
		lineSwipe2.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		compoundLineSwipe.AddLine(lineSwipe2);
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, 0.5f, lineSwipe2, 0, 0.5f));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe, 0, LineRelationship.LinePosition.BetweenLeftRight, lineSwipe2, 0, 0.5f));
		compoundLineSwipe.AddRelationship(new LineRelationship(lineSwipe2, 0, LineRelationship.LinePosition.BetweenTopBottom, lineSwipe, 0, 0.5f));
		lineSwipes[19] = compoundLineSwipe;
		lineSwipe = new LineSwipe(LineType.Minus.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipes[20] = lineSwipe;
		lineSwipe = new LineSwipe(LineType.RightCheck.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus135, 1, Segment.SizeSpec.Smallest));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Plus45, 1, Segment.SizeSpec.Bigger));
		lineSwipes[21] = lineSwipe;
		lineSwipe = new LineSwipe(LineType.LeftCheck.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus135, 1, Segment.SizeSpec.Smallest));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Minus45, 1, Segment.SizeSpec.Bigger));
		lineSwipes[22] = lineSwipe;
		lineSwipe = new LineSwipe(LineType.Square.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 1, Segment.SizeSpec.Ratio));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 1, Segment.SizeSpec.Ratio));
		lineSwipe.closed = true;
		lineSwipes[23] = lineSwipe;
		lineSwipe = new LineSwipe(LineType.Rectangle.ToString());
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Down, 0, Segment.SizeSpec.Ignore));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Right, 0, Segment.SizeSpec.Ignore));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Up, 0, Segment.SizeSpec.Ignore));
		lineSwipe.AddSegment(new Segment(SwipeGesture.SwipeDirection.Left, 0, Segment.SizeSpec.Ignore));
		lineSwipe.closed = true;
		lineSwipe.maintainAspectRatio = false;
		lineSwipes[24] = lineSwipe;
	}

	public static LineFactory Factory()
	{
		if (_factory == null)
		{
			_factory = new LineFactory();
		}
		return _factory;
	}

	public bool IsCompound(LineType lineType)
	{
		return lineSwipes[(int)lineType].IsCompound();
	}

	public int GetCount(LineType lineType)
	{
		return lineSwipes[(int)lineType].Count();
	}

	public int GetMaxSegmentCount(LineType lineType)
	{
		return lineSwipes[(int)lineType].GetMaxSegment();
	}

	public LineSwipeBase GetLineSwipe(LineType lineType)
	{
		return lineSwipes[(int)lineType];
	}

	public LineType FindLineType(LineType[] possibleTypes, List<SwipeSegmentList> swipeList, LineGesture.LineIdentification lineIdentification, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent)
	{
		if (lineSwipes == null || lineSwipes.Length == 0)
		{
			Debug.LogError("LineFactory:FindLineType *** ERROR *** LineFactory not initialized");
			return LineType.None;
		}
		LineType lineType = doFindLineType(possibleTypes, swipeList, LineGesture.LineIdentification.Precise, restrictLineSwipeDirection, positionDiff, lengthDiffPercent);
		if (lineType != LineType.None)
		{
			return lineType;
		}
		if (lineIdentification == LineGesture.LineIdentification.Clean || lineIdentification == LineGesture.LineIdentification.Sloppy)
		{
			lineType = doFindLineType(possibleTypes, swipeList, LineGesture.LineIdentification.Clean, restrictLineSwipeDirection, positionDiff, lengthDiffPercent);
			if (lineType != LineType.None)
			{
				return lineType;
			}
		}
		if (lineIdentification == LineGesture.LineIdentification.Sloppy)
		{
			lineType = doFindLineType(possibleTypes, swipeList, LineGesture.LineIdentification.Sloppy, restrictLineSwipeDirection, positionDiff, lengthDiffPercent);
			if (lineType != LineType.None)
			{
				return lineType;
			}
		}
		return LineType.None;
	}

	private LineType doFindLineType(LineType[] possibleTypes, List<SwipeSegmentList> swipeList, LineGesture.LineIdentification lineIdentification, LineGesture.LineSwipeDirection restrictLineSwipeDirection, float positionDiff, float lengthDiffPercent)
	{
		for (int i = 0; i < possibleTypes.Length; i++)
		{
			if (lineSwipes[(int)possibleTypes[i]] == null)
			{
				Debug.LogError(string.Concat("LineFactory:FindLineType *** ERROR *** LineFactory item ", possibleTypes[i], " is not defined."));
				return LineType.None;
			}
			if (lineSwipes[(int)possibleTypes[i]].Compare(swipeList, restrictLineSwipeDirection, positionDiff, lengthDiffPercent, lineIdentification))
			{
				return possibleTypes[i];
			}
		}
		return LineType.None;
	}
}
