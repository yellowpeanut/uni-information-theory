using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crypt5
{
    class GenPrime
    {

        public GenPrime()
        {

        }

        public ulong Generate(int n)
        {
            ulong number = getLowLevelPrime(n);
            while (!isMillerRabinPassed(number))
                number = getLowLevelPrime(n);
            return number;
        }

        ulong nBitRandom(int n)
        {
            var rand = new Random();

            // Returns a random number
            // between 2**(n-1)+1 and 2**n-1'''
            ulong max = (ulong)Math.Pow(2, n) - 1;
            ulong min = (ulong)Math.Pow(2, n - 1) + 1;
            return Next(min, max + 1);
        }

        ulong getLowLevelPrime(int n)
        {
            ulong[] first_primes_list = new ulong[] { 1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37 };

            // Generate a prime candidate divisible
            // by first primes

            //  Repeat until a number satisfying
            //  the test isn't found
            while (true)
            {
                //  Obtain a random number
                ulong prime_candidate = nBitRandom(n);

                foreach (ulong divisor in first_primes_list)
                {
                    if (prime_candidate % divisor == 0
                        && divisor * divisor <= prime_candidate)
                        break;
                    //  If no divisor found, return value
                    else
                        return prime_candidate;
                }
            }
        }

        // This function calculates (base ^ exp) % mod
        ulong expmod(ulong _base, int exp, ulong mod)
        {
            if (exp == 0) return 1;
            if (exp % 2 == 0)
            {
                return (ulong)Math.Pow(expmod(_base, (exp / 2), mod), 2) % mod;
            }
            else
            {
                return (_base * expmod(_base, (exp - 1), mod)) % mod;
            }
        }

        bool trialComposite(ulong round_tester, int evenComponent,
                                   ulong miller_rabin_candidate, int maxDivisionsByTwo)
        {
            if (expmod(round_tester, evenComponent, miller_rabin_candidate) == 1)
                return false;
            for (int i = 0; i < maxDivisionsByTwo; i++)
            {
                if (expmod(round_tester, (1 << i) * evenComponent,
                           miller_rabin_candidate) == miller_rabin_candidate - 1)
                    return false;
            }
            return true;
        }

        bool isMillerRabinPassed(ulong miller_rabin_candidate)
        {
            // Run 20 iterations of Rabin Miller Primality test

            int maxDivisionsByTwo = 0;
            int evenComponent = (int)miller_rabin_candidate - 1;

            while (evenComponent % 2 == 0)
            {
                evenComponent >>= 1;
                maxDivisionsByTwo += 1;
            }

            // Set number of trials here
            int numberOfRabinTrials = 20;
            for (int i = 0; i < (numberOfRabinTrials); i++)
            {
                ulong round_tester = Next(2, miller_rabin_candidate);

                if (trialComposite(round_tester, evenComponent,
                                   miller_rabin_candidate, maxDivisionsByTwo))
                    return false;
            }
            return true;
        }
        public ulong Next(ulong min, ulong max)
        {
            System.Random rd = new System.Random();

            if (max <= min)
                throw new ArgumentOutOfRangeException("max", "max must be > min!");

            ulong uRange = (max - min);
            ulong ulongRand;
            do
            {
                byte[] buf = new byte[8];
                rd.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (ulongRand % uRange) + min;

        }
    }
}
