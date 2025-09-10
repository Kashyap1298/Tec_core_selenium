#!/usr/bin/env python
"""
Selenium Login Test Runner
A Python wrapper to run the C# Selenium login test
"""

import subprocess
import sys
import os
from datetime import datetime

def run_login_test(email=None, password=None):
    """
    Run the Selenium login test with optional credentials
    """
    # Change to the test directory
    test_dir = os.path.join(os.path.dirname(__file__), "SeleniumLoginTest")
    os.chdir(test_dir)
    
    # Build the command
    cmd = ["dotnet", "run"]
    
    # Add credentials if provided
    if email and password:
        cmd.extend(["--", email, password])
    
    print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] Starting Selenium Login Test...")
    print("-" * 50)
    
    try:
        # Run the test
        result = subprocess.run(cmd, capture_output=False, text=True)
        
        if result.returncode == 0:
            print("-" * 50)
            print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] Test completed successfully!")
        else:
            print("-" * 50)
            print(f"[{datetime.now().strftime('%Y-%m-%d %H:%M:%S')}] Test failed with exit code: {result.returncode}")
            
        return result.returncode
        
    except FileNotFoundError:
        print("Error: .NET SDK not found. Please ensure .NET is installed.")
        return 1
    except Exception as e:
        print(f"Error running test: {e}")
        return 1

def main():
    """
    Main entry point
    """
    if len(sys.argv) == 3:
        # Custom credentials provided
        email = sys.argv[1]
        password = sys.argv[2]
        print(f"Using provided credentials: {email}")
        return run_login_test(email, password)
    elif len(sys.argv) == 1:
        # Use default credentials
        print("Using default credentials from LoginTest.cs")
        return run_login_test()
    else:
        print("Usage:")
        print("  python run_selenium_test.py                    # Use default credentials")
        print("  python run_selenium_test.py email password     # Use custom credentials")
        return 1

if __name__ == "__main__":
    sys.exit(main())