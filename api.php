<?php

abstract class Enum {
    private static $constCacheArray = NULL;

    private static function getConstants() {
        if (self::$constCacheArray == NULL) {
            self::$constCacheArray = [];
        }
        $calledClass = get_called_class();
        if (!array_key_exists($calledClass, self::$constCacheArray)) {
            $reflect = new ReflectionClass($calledClass);
            self::$constCacheArray[$calledClass] = $reflect->getConstants();
        }
        return self::$constCacheArray[$calledClass];
    }

    public static function isValidName($name, $strict = false) {
        $constants = self::getConstants();

        if ($strict) {
            return array_key_exists($name, $constants);
        }

        $keys = array_map('strtolower', array_keys($constants));
        return in_array(strtolower($name), $keys);
    }

    public static function GetName($name){
        $constants = self::getConstants();
        return array_search($name, $constants);
    }
    public static function isValidValue($value, $strict = true) {
        $values = array_values(self::getConstants());
        return in_array($value, $values, $strict);
    }
}


abstract class CsharpEnum extends Enum {
    const Unknown = 0;
    const Intern = 1;
    const Employee = 2;
    const Administration = 3;
    const Root = 4;
}
$Account = [
    "Username1" => ["Password1", CsharpEnum::Intern]
];

if (isset($_POST['user']) && isset($_POST['password'])){
    if (isset($Account[$_POST['user']]) && $Account[$_POST['user']][0] == $_POST['password'])
        exit($Account[$_POST['user']][1]."");
    
    exit(CsharpEnum::Unknown."");
}
