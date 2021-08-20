using System;

namespace CSharpPractice
{
    

    class Program
    {
        private static void Calculator() {
            int num01;
            int num02;
            int answer;
            string operand;

            Console.Write("첫 번째 숫자:");
            num01 = Convert.ToInt32(Console.ReadLine());
            Console.Write("연산자:");
            operand = Console.ReadLine();
            Console.Write("두 번째 숫자:");
            num02 = Convert.ToInt32(Console.ReadLine());

            if (operand == "+")
            {
                answer = num01 + num02;
                Console.WriteLine(num01 + operand + num02 + "=" + answer);
            }
            else if (operand == "-")
            {
                answer = num01 - num02;
                Console.WriteLine(num01 + operand + num02 + "=" + answer);
            }
            else if (operand == "*")
            {
                answer = num01 * num02;
                Console.WriteLine(num01 + operand + num02 + "=" + answer);
            }
            else if (operand == "/")
            {
                answer = num01 / num02;
                Console.WriteLine(num01 + operand + num02 + "=" + answer);
            }
            else
            {
                Console.WriteLine("지원하지 않는 연산자입니다.");
            }
        }

        private static void GradeChecker() {
            int grade;

            Console.Write("이번 시험 성적을 입력하세요:");
            grade = Convert.ToInt32(Console.ReadLine());
            if (grade <= 100 && grade >= 90)
            {
                Console.WriteLine("축하합니다! A입니다.");
            }
            else if (grade >= 80)
            {
                Console.WriteLine("나쁘지 않은데요, B입니다.");
            }
            else if (grade >= 70)
            {
                Console.WriteLine("다음엔 더 잘 할 수 있어요, C입니다.");
            }
            else if (grade >= 60)
            {
                Console.WriteLine("평균은 넘은 것 같네요. D입니다.");
            }
            else if (grade >= 0)
            {
                Console.WriteLine("노력이 많이 필요합니다.. F입니다.");
            }
            else {
                Console.WriteLine("잘못된 점수가 입력되었습니다.");
            }
        }
        
        private static void for_practice() {
            for (int i = 1; i <= 10; i++) {
                Console.Write(i.ToString() + " ");    
            }
            Console.WriteLine(" Done.");
            for (int j = 20; j > 10; j--) {
                Console.Write(j.ToString() + " ");
            }
            Console.WriteLine(" Done.");
            for (int k = 10; k <= 100; k += 10) {
                Console.Write(k.ToString() + " ");
            }
            Console.WriteLine(" Done.");
        }

        static void Main(string[] args)
        {

            Calculator();
            GradeChecker();
            for_practice();
            Console.ReadKey();
        }
    }
}
