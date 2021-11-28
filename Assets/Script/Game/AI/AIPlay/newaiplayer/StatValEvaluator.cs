using System;
using System.Collections;
using UnityEngine;

public class StatValEvaluator
{
	public const int WIN = 1000000;
	public const int WINBORDER = 900000;
	public const int UNDEF = -10000000;
	public const int OWNTHREATBONUS = 256;
	public const int OPPTHREATBONUS = 256;

	public int[] statValues;
	InterestingFieldAgent fieldagent;

	public StatValEvaluator(InterestingFieldAgent ifa)
	{
		statValues = new int[262144];
		generateStatValues();
		fieldagent = ifa;

	}
	
	private void generateStatValues()
	{
		Console.WriteLine("Generating static values: start");

		for (int i = 0; i < 262144; ++i) {

			int digit = i;
			
			int left = 0;
			int right = 0;
			int ownposs = 0;
			int ownrow = 0;
			int opp_cutoff = 0;
			int win = 0;
			int oppl = 0;
			int oppr = 0;
			int ownlpos = 4;
			int ownrpos = 4;
			int opplstop = 0;
			int opprstop = 0;
			int ownbolpos = -1;
			int ownborpos = 9;
			int leftpos = 4;
			int rightpos = 4;
			int opplpos = 4;
			int opprpos = 4;

			bool leftinclown = false;
			bool rightinclown = false;
			
			// free Field =  1
			// our Field =   2
			// opp Field =   0
			// nonexist Field = 3
			int[] line = new int[9];
			for (int y = 0; y < 9; ++y) {
				line[y] = digit % 4;
				digit >>= 2;
			}
			for (int ii = 4; ii >= 0; --ii) {
				if (left == 0 && oppl == 0 && line[ii] == 2) {
					++ownrow;
					ownlpos = ii;
				}
				if (oppl == 0 && line[ii] == 1) {
					++left;
					leftpos = ii;
				}
				if (left != 0 && oppl == 0 && line[ii] == 2) {
					leftinclown = true;
					++left;
					leftpos = ii;
				}
				if (line[ii] == 0 && opplstop == 0)
					++oppl;
				if ((line[ii] == 1 || line[ii] == 2) && oppl != 0) 
					opplstop = 1;
				if (line[ii] == 3) 
					break;
			}
			for (int ii = 4; ii < 9; ++ii) {
				if (right == 0 && oppr == 0 && line[ii] == 2) {
					++ownrow;
					ownrpos = ii;
				}
				if (oppr == 0 && line[ii] == 1) {
					++right;
					rightpos = ii;
				}
				if (right != 0 && oppr == 0 && line[ii] == 2) {
					rightinclown = true;
					++right;
					rightpos = ii;
				}
				if (line[ii] == 0 && opprstop == 0) ++oppr;
				if ((line[ii] == 1 || line[ii] == 2) && oppr != 0) opprstop = 1;
				if (line[ii] == 3) break;
			}

			for (int ii = 3; ii >= 0; --ii) 
			{
				if (line[ii] == 1 || line[ii] == 0)
					continue;
				if (line[ii] == 2 || line[ii] == 3)
				{
					ownbolpos = ii;
					break;
				}
			}
			for (int ii = 5; ii < 9; ++ii) 
			{
				if (line[ii] == 1 || line[ii] == 0)
					continue;
				if (line[ii] == 2 || line[ii] == 3)
				{
					ownborpos = ii;
					break;
				}
			}

			if (oppl != 0) opplpos = leftpos - 1;
			if (oppr != 0) opprpos = rightpos + 1;
			--ownrow;

			//numbers are initialized to 0
			//oppl now contains number of opponent stones in a row left of the middle.
			//left contains the number of free or our fields left of the ownrow.
			//ownrow now contains the length of our row of stones over the middle
			//right contains the number of free or our fields right of the ownrow.
			//oppr now contains number of opponent stones in a row right of the middle.
			//
			//positions are initialized to 4
			//ownlpos contains the index of our leftmost piece in the ownrow.
			//ownrpos contains the index of our rightmost piece in the ownrow.
			//ownbolpos contains the index of our[or nonexistent] rightmost piece left of the opponent row.
			//ownborpos contains the index of our[or nonexistent leftmost piece right of the opponent row.
			//leftpos contains the index of the leftmost piece in left.
			//rightpos contains the index of the rightmost piece in right.
			//opplpos contains the first opponent piece on the left side.
			//opprpos contains the first opponent piece on the right side.
			//
			//leftinclown is true if left includes an own piece.
			//rightinclown is true if right includes an own piece.
			if (left + ownrow + right >= 5) ownposs = (left + ownrow + right) - 4;
			
			int ownrowbonus = 0;
			if (ownposs > 0) ownrowbonus = 8 * ownrow;

			if (ownborpos - ownbolpos > 5 && 
				(opplpos != 4 || opprpos != 4) &&
				(opplpos > ownbolpos || opprpos < ownborpos))
				opp_cutoff = 5 - (4 - opplpos) + 5 - (opprpos - 4);
			if (ownrow >= 5) win = WIN;

			
			statValues[i] = 2 * opp_cutoff + ownrowbonus + win;

//			statValues[i] = /*2 * ownposs + */4 * opp_cutoff  + ownrowbonus + win;
			/*
			if (line[4] == 2) {
				Console.Write("Generating static values: key: {0}, string: ", i);
				for (int y = 0; y < 9; ++y) {
					Console.Write(line[y] == 1?".":line[y]==2?"X":line[y]==3?"#":line[y]==0?"O":"");
				}
				Console.WriteLine("   , val: {0}, ownposs: {1}, opp_cutoff: {2}", statValues[i], ownposs, opp_cutoff);
			}
			*/
			
		}
		Console.WriteLine("Generating static values: finished");
	}

	public int smstatVal(int[,] board, Coordinate node, int attacker, int userX, int userY, ArrayList white_illegal_move_list)
	{
		//Console.WriteLine("statVal�� �Է� ��: board=> " + board + ", node=> " + node + ", attacker=> " + attacker);
		int turn = attacker;
		if (turn != board[node.X, node.Y]) throw new Exception();
		int init = board[node.X, node.Y];
		board[node.X, node.Y] = 0;
		// free Field =  1
		// our Field =   2
		// opp Field =   0
		// nonexist Field = 3
		int tmpval = 0;
		int digit1 = 0;
		int digit2 = 0;
		int digit3 = 0;
		int digit4 = 0;
		//ó��  node�� ���� �ް� �� �ֺ��� �ִ� ���� ã�´�. 1234*1234
		bool digit1_1 = false;
		bool digit2_1 = false;
		bool digit3_1 = false;
		bool digit4_1 = false;
		//�� ���ݱǿ� �ִµ�
		int mydigit1 = 0;
		int mydigit2 = 0;
		int mydigit3 = 0;
		int mydigit4 = 0;

		for (int j = 0; j < 9; ++j)
		{
			if (j + node.X - 4 >= 0 && j + node.X - 4 < 15)//�¿�� ���Ǳ�
			{
				digit1 += board[node.X + j - 4, node.Y] * turn + 1;
                if ((board[node.X + j - 4, node.Y] * turn) == 1)
                {
					if (digit1_1 == true)
                    {
						digit1 += board[node.X + j - 4, node.Y] * turn + 2;
                    }
                    else
                    {
						digit1_1 = true;
					}

					mydigit1 = 0;
				}
                else
                {
					digit1_1 = false;

					if((board[node.X + j - 4, node.Y] * turn) == -1)
                    {
						mydigit1 += 1;
						if (mydigit1 == 4)
						{
							return -1000;
						}
					}
				}
				//Console.WriteLine("Grabbing {0}/{1}", node.X + j - 4, node.Y);
				//Console.WriteLine("digit1=> "+ digit1);
			}
			else
			{
				digit1 += 1;
			}
			//=================
			if (j + node.Y - 4 >= 0 && j + node.Y - 4 < 15)//�� �Ʒ��� ���Ǳ�
			{
				digit2 += board[node.X, node.Y + j - 4] * turn + 1;
				//Console.WriteLine("Grabbing {0}/{1}", node.X, node.Y + j -4);
				//Console.WriteLine("digit2=> " + digit2);
				if ((board[node.X, node.Y + j - 4] * turn) == 1)
				{
					if (digit2_1 == true)
					{
						digit2 += board[node.X, node.Y + j - 4] * turn + 2;
					}
					else
					{
						digit2_1 = true;
					}

					mydigit2 = 0;
				}
				else
				{
					digit2_1 = false;

					if((board[node.X, node.Y + j - 4] * turn) == -1)
                    {
						mydigit2 += 1;
						if (mydigit2 == 4)
						{
							return -1000;
						}
					}
				}
			}
			else
			{
				digit2 += 1;
			}
			//=================
			if (node.X + j - 4 >= 0 && node.X + j - 4 < 15 &&
				node.Y + j - 4 >= 0 && node.Y + j - 4 < 15)//
			{
				digit3 += board[node.X + j - 4, node.Y + j - 4] * turn + 1;
				//Console.WriteLine("Grabbing {0}/{1}", node.X + j - 4, node.Y + j - 4);
				//Console.WriteLine("digit3=> " + digit3);
				if ((board[node.X + j - 4, node.Y + j - 4] * turn) == 1)
				{
					if (digit3_1 == true)
					{
						digit3 += board[node.X + j - 4, node.Y + j - 4] * turn + 2;
					}
					else
					{
						digit3_1 = true;
					}

					mydigit3 = 0;
				}
				else
				{
					digit3_1 = false;

					if((board[node.X + j - 4, node.Y + j - 4] * turn) == -1)
                    {
						mydigit3 += 1;
						if (mydigit3 == 4)
						{
							return -1000;
						}
					}
				}
			}
			else
			{
				digit3 += 1;
			}
			//=================
			if (node.X - j + 4 >= 0 && node.X - j + 4 < 15 &&
				node.Y + j - 4 >= 0 && node.Y + j - 4 < 15)
			{
				digit4 += board[node.X - j + 4, node.Y + j - 4] * turn + 1;
				//Console.WriteLine("Grabbing {0}/{1}", node.X - j + 4, node.Y + j - 4);
				//Console.WriteLine("digit4=> " + digit4);
				if ((board[node.X - j + 4, node.Y + j - 4] * turn) == 1)
				{
					if (digit4_1 == true)
					{
						digit4 += board[node.X - j + 4, node.Y + j - 4] * turn + 2;
					}
					else
					{
						digit4_1 = true;
					}

					mydigit4 = 0;
				}
				else
				{
					digit4_1 = false;

					if((board[node.X - j + 4, node.Y + j - 4] * turn) == -1)
                    {
						mydigit4 += 1;
						if (mydigit4 == 4)
						{
							return -1000;
						}
					}
				}
			}
			else
			{
				digit4 += 1;
			}
		}

		board[node.X, node.Y] = init;
		//Console.WriteLine("Value for horizontal row: {0}", 3 * digit1);
		//Console.WriteLine("Value for vertical row: {0}", 3 * digit2);
		//Console.WriteLine("Value for diag1 row: {0}", 3 * digit3);
		//Console.WriteLine("Value for diag2 row: {0}", 3 * digit4);
		
		tmpval += 3 * digit1;
		tmpval += 3 * digit2;
		tmpval += 3 * digit3;
		tmpval += 3 * digit4;

		if(userX-2 < node.X && node.X  < userX + 2)
        {
			if (userY - 2 < node.Y && node.Y < userY + 2)
			{
				//Console.WriteLine("�ֺ� �ʵ�");
				tmpval += 2;
			}
		}

		//Debug.Log("AI_IQ=> " + DataManager.instance.AI_IQ);
		if (DataManager.instance.AI_IQ > 1)
		{
			if (white_illegal_move_list.Count != 0)
			{
				Debug.Log("white_illegal_move_list");
				try
				{
					foreach (Coordinate field in white_illegal_move_list)
					{
						if (field.X == node.X && field.Y == node.Y)
						{
							tmpval += 50;

						}
					}
				}
				catch (Exception e)
				{
					Debug.Log(e.ToString());
				}
			}
		}

		//Debug.Log("statVal�� ��ȯ ��=> " + (tmpval * turn)+ "tmpval=> "+ tmpval+ ", turn=>" + turn);
		return (tmpval * turn);
	}


	/** Computes the static value of a MoveNode.
	 *
	 * @param board The actual board.
	 * @param node The node to be rated
	 * @returns the static value of the Node.
	 */
	public int statVal(int[,] board, Coordinate node, int attacker, ArrayList white_illegal_move_list)
	{
		//Console.WriteLine("statVal�� �Է� ��: board=> " + board + ", node=> " + node + ", attacker=> " + attacker);
		int turn = attacker;
		if (turn != board[node.X, node.Y]) throw new Exception();

		// free Field =  1
		// our Field =   2
		// opp Field =   0
		// nonexist Field = 3
		int tmpval = 0;
		int digit1 = 0;
		int digit2 = 0;
		int digit3 = 0;
		int digit4 = 0;
		// * * * * V * * * *
		for (int j = 0; j < 9; ++j) 
		{
			if (j + node.X - 4 >= 0 && j + node.X - 4 < 15)
			{
				digit1 += board[node.X + j - 4, node.Y] * turn + 1;
				//Console.WriteLine("Grabbing {0}/{1}", node.X + j - 4, node.Y);
				//Console.WriteLine("digit1=> "+ digit1);
			}
			else
			{
				digit1 += 3;
			}
			//=================
			if (j + node.Y - 4 >= 0 && j + node.Y - 4 < 15)
			{
				digit2 += board[node.X, node.Y + j - 4] * turn + 1;
				//Console.WriteLine("Grabbing {0}/{1}", node.X, node.Y + j -4);
				//Console.WriteLine("digit2=> " + digit2);
			}
			else 
			{ 
				digit2 += 3; 
			}
			//=================
			if (node.X + j - 4 >= 0 && node.X + j - 4 < 15 &&
				node.Y + j - 4 >= 0 && node.Y + j - 4 < 15)
			{
				digit3 += board[node.X + j - 4, node.Y + j - 4] * turn + 1;
				//Console.WriteLine("Grabbing {0}/{1}", node.X + j - 4, node.Y + j - 4);
				//Console.WriteLine("digit3=> " + digit3);
			}
			else
			{
				digit3 += 3;
			}
			//=================
			if (node.X - j + 4 >= 0 && node.X - j + 4 < 15 &&
				node.Y + j - 4 >= 0 && node.Y + j - 4 < 15)
			{
				digit4 += board[node.X - j + 4, node.Y + j - 4] * turn + 1;
				//Console.WriteLine("Grabbing {0}/{1}", node.X - j + 4, node.Y + j - 4);
				//Console.WriteLine("digit4=> " + digit4);
			}
			else
			{
				digit4 += 3;
			}
			//=================
			if (j != 8) 
			{
				digit1 <<= 2;
				digit2 <<= 2;
				digit3 <<= 2;
				digit4 <<= 2;
			}
		}
		
		/*
		Console.WriteLine("Value for horizontal row: {0}", statValues[digit1]);
		Console.WriteLine("Value for vertical row: {0}", statValues[digit2]);
		Console.WriteLine("Value for diag1 row: {0}", statValues[digit3]);
		Console.WriteLine("Value for diag2 row: {0}", statValues[digit4]);
		*/
		tmpval += statValues[digit1];
		tmpval += statValues[digit2];
		tmpval += statValues[digit3];
		tmpval += statValues[digit4];


		if (white_illegal_move_list.Count != 0)
		{
			//Debug.Log("white_illegal_move_list");
			try
			{
				foreach (Coordinate field in white_illegal_move_list)
				{
					if (field.X == node.X && field.Y == node.Y)
					{
						tmpval += 50;

					}
				}
			}
			catch (Exception e)
			{
				Debug.Log(e.ToString());
			}
		}

		// look up the threat-situation
		int ownthreatbonus = 0;
		int oppthreatbonus = 0;

		foreach (Threat t in fieldagent.ownaddedthreatlist)
		{
			if (t.create == false) ownthreatbonus += 3 - t.category;
		}
		if (attacker == 1)
		{
			foreach (Threat t in fieldagent.oppremovedthreatlist)
			{
				if (t.create == false) ownthreatbonus += 3 - t.category;
			}
		}
		
		foreach (Threat t in fieldagent.oppaddedthreatlist)
		{
			if (t.create == false) oppthreatbonus += 3 - t.category;
		}
		if (attacker == -1)
		{
			foreach (Threat t in fieldagent.ownremovedthreatlist)
			{
				if (t.create == false) oppthreatbonus += 3 - t.category;
			}
		}
		
		int bonus = OWNTHREATBONUS * ownthreatbonus - OPPTHREATBONUS * oppthreatbonus;
		//Console.WriteLine("statVal�� ��ȯ ��=> " + (bonus + (tmpval * turn)));
		return bonus + (tmpval * turn);

	}
}
