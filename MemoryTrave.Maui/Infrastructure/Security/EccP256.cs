using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MemoryTrave.Maui.Infrastructure.Security;

public class EccP256
{
    private static readonly ECDomainParameters DomainParams;
    
    static EccP256()
    {
        var curve = SecNamedCurves.GetByName("secp256r1");
        DomainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
    }
    
    public static AsymmetricCipherKeyPair GenerateKeyPair()
    {
        var keyGen = new ECKeyPairGenerator();
        keyGen.Init(new ECKeyGenerationParameters(DomainParams, new SecureRandom()));
        var receiverKeyPair = keyGen.GenerateKeyPair();
        return receiverKeyPair;
    }
    
    public static byte[] Encrypt(AsymmetricKeyParameter publicKey, byte[] data)
    {
        var curve = ((ECPublicKeyParameters)publicKey).Parameters;
        var keyGen = new ECKeyPairGenerator();
        keyGen.Init(new ECKeyGenerationParameters(curve, new SecureRandom()));
        var ephKeyPair = keyGen.GenerateKeyPair();

        var agreement = new ECDHBasicAgreement();
        var kdf = new Kdf2BytesGenerator(new Sha256Digest());
        var mac = new HMac(new Sha256Digest());
        var engine = new IesEngine(agreement, kdf, mac);

        var iesParams = new IesWithCipherParameters(new byte[0], new byte[0], 128, 128);

        engine.Init(true, ephKeyPair.Private, publicKey, iesParams);

        var cipher = engine.ProcessBlock(data, 0, data.Length);

        var ephPubEncoded = ephKeyPair.Public is ECPublicKeyParameters pub
            ? pub.Q.GetEncoded(false)
            : throw new Exception("Invalid ephemeral public key");

        var result = new byte[ephPubEncoded.Length + cipher.Length];
        Buffer.BlockCopy(ephPubEncoded, 0, result, 0, ephPubEncoded.Length);
        Buffer.BlockCopy(cipher, 0, result, ephPubEncoded.Length, cipher.Length);

        return result;
    }

    public static byte[] Decrypt(AsymmetricKeyParameter privateKey, byte[] cipherData)
    {
        var curve = ((ECPrivateKeyParameters)privateKey).Parameters;
        var keySize = (curve.Curve.FieldSize + 7) / 8 * 2 + 1;

        var ephPubEncoded = new byte[keySize];
        Buffer.BlockCopy(cipherData, 0, ephPubEncoded, 0, keySize);
        var ephPubKey = new ECPublicKeyParameters(curve.Curve.DecodePoint(ephPubEncoded), curve);

        var cipher = new byte[cipherData.Length - keySize];
        Buffer.BlockCopy(cipherData, keySize, cipher, 0, cipher.Length);

        var agreement = new ECDHBasicAgreement();
        var kdf = new Kdf2BytesGenerator(new Sha256Digest());
        var mac = new HMac(new Sha256Digest());
        var engine = new IesEngine(agreement, kdf, mac);

        var iesParams = new IesWithCipherParameters(new byte[0], new byte[0], 128, 128);

        engine.Init(false, privateKey, ephPubKey, iesParams);

        return engine.ProcessBlock(cipher, 0, cipher.Length);
    }
    
    public static byte[]? PublicKeyToBytes(AsymmetricKeyParameter publicKey) =>
        publicKey is not ECPublicKeyParameters pubKey ? null : pubKey.Q.GetEncoded(false);
    
    public static byte[]? PrivateKeyToBytes(AsymmetricKeyParameter privateKey) =>
        privateKey is not ECPrivateKeyParameters privKey ? null : privKey.D.ToByteArrayUnsigned();
    
    public static ECPublicKeyParameters StringToPublicKey(string publicKeyString)
    {
        var publicKeyBytes = Convert.FromBase64String(publicKeyString);
        var q = DomainParams.Curve.DecodePoint(publicKeyBytes);
        return new ECPublicKeyParameters(q, DomainParams);
    }
    
    public static ECPrivateKeyParameters StringToPrivateKey(string privateKeyString)
    {
        var privateKeyBytes = Convert.FromBase64String(privateKeyString);
        var d = new Org.BouncyCastle.Math.BigInteger(1, privateKeyBytes);
        return new ECPrivateKeyParameters(d, DomainParams);
    }
}